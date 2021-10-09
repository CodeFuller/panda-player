using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositePlaylistAdviser : ICompositePlaylistAdviser
	{
		private const string PlaylistAdviserDataKey = "PlaylistAdviserData";

		private readonly IDiscGrouper discGrouper;
		private readonly IPlaylistAdviser rankBasedAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favoriteAdviseGroupsAdviser;
		private readonly ISessionDataService sessionDataService;
		private readonly AdviserSettings settings;

		private PlaylistAdviserMemo Memo { get; set; }

		public CompositePlaylistAdviser(IDiscGrouper discGrouper, IPlaylistAdviser rankBasedAdviser, IPlaylistAdviser highlyRatedSongsAdviser,
			IPlaylistAdviser favoriteAdviseGroupsAdviser, ISessionDataService sessionDataService, IOptions<AdviserSettings> options)
		{
			this.discGrouper = discGrouper ?? throw new ArgumentNullException(nameof(discGrouper));
			this.rankBasedAdviser = rankBasedAdviser ?? throw new ArgumentNullException(nameof(rankBasedAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favoriteAdviseGroupsAdviser = favoriteAdviseGroupsAdviser ?? throw new ArgumentNullException(nameof(favoriteAdviseGroupsAdviser));
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, int requiredAdvisesCount, CancellationToken cancellationToken)
		{
			Memo ??= await sessionDataService.GetData<PlaylistAdviserMemo>(PlaylistAdviserDataKey, cancellationToken) ?? CreateDefaultMemo();
			var playbacksMemo = Memo;

			var adviseGroups = (await discGrouper.GroupLibraryDiscs(discs, cancellationToken)).ToList();
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var highlyRatedSongsAdvises = await highlyRatedSongsAdviser.Advise(adviseGroups, playbacksInfo, cancellationToken);
			var favoriteAdviseGroupAdvises = await favoriteAdviseGroupsAdviser.Advise(adviseGroups, playbacksInfo, cancellationToken);
			var rankedAdvises = await rankBasedAdviser.Advise(adviseGroups, playbacksInfo, cancellationToken);

			var playlistQueue = new CompositeAdvisedPlaylistQueue(highlyRatedSongsAdvises, favoriteAdviseGroupAdvises, rankedAdvises);

			var advisedPlaylists = new List<AdvisedPlaylist>(requiredAdvisesCount);

			var knownAdviseSets = new HashSet<string>();
			while (advisedPlaylists.Count < requiredAdvisesCount)
			{
				var currentAdvise = GetNextAdvisedPlaylist(playbacksMemo, playlistQueue);
				if (currentAdvise == null)
				{
					break;
				}

				var adviseSet = currentAdvise.AdviseSet;
				if (adviseSet != null)
				{
					if (knownAdviseSets.Contains(adviseSet.Id))
					{
						continue;
					}

					knownAdviseSets.Add(adviseSet.Id);
				}

				advisedPlaylists.Add(currentAdvise);
				playbacksMemo = playbacksMemo.RegisterPlayback(currentAdvise);
			}

			return advisedPlaylists;
		}

		private AdvisedPlaylist GetNextAdvisedPlaylist(PlaylistAdviserMemo playbacksMemo, CompositeAdvisedPlaylistQueue playlistQueue)
		{
			if (playbacksMemo.PlaybacksSinceHighlyRatedSongsPlaylist + 1 >= settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs &&
			    playlistQueue.TryDequeueHighlyRatedSongsAdvise(out var currentAdvise))
			{
				return currentAdvise;
			}

			if (playbacksMemo.PlaybacksSinceFavoriteAdviseGroup + 1 >= settings.PlaybacksBetweenFavoriteAdviseGroups &&
			    playlistQueue.TryDequeueFavoriteAdviseGroupAdvise(out currentAdvise))
			{
				return currentAdvise;
			}

			return playlistQueue.TryDequeueRankedAdvise(out currentAdvise) ? currentAdvise : null;
		}

		private PlaylistAdviserMemo CreateDefaultMemo()
		{
			// If no previous PlaylistAdviserMemo exist, we're initializing memo with threshold values so that promoted advises go first.
			return new(settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs, settings.PlaybacksBetweenFavoriteAdviseGroups);
		}

		public async Task RegisterAdvicePlayback(AdvisedPlaylist advise, CancellationToken cancellationToken)
		{
			Memo = Memo.RegisterPlayback(advise);
			await sessionDataService.SaveData(PlaylistAdviserDataKey, Memo, cancellationToken);
		}
	}
}
