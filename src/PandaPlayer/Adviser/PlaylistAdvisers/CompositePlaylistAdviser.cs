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
		private readonly IPlaylistAdviser rankedDiscsAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favoriteArtistAdviser;
		private readonly ISessionDataService sessionDataService;
		private readonly AdviserSettings settings;

		private PlaylistAdviserMemo Memo { get; set; }

		public CompositePlaylistAdviser(IDiscGrouper discGrouper, IPlaylistAdviser rankedDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser,
			IPlaylistAdviser favoriteArtistAdviser, ISessionDataService sessionDataService, IOptions<AdviserSettings> options)
		{
			this.discGrouper = discGrouper ?? throw new ArgumentNullException(nameof(discGrouper));
			this.rankedDiscsAdviser = rankedDiscsAdviser ?? throw new ArgumentNullException(nameof(rankedDiscsAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favoriteArtistAdviser = favoriteArtistAdviser ?? throw new ArgumentNullException(nameof(favoriteArtistAdviser));
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
			var favoriteArtistDiscsAdvises = await favoriteArtistAdviser.Advise(adviseGroups, playbacksInfo, cancellationToken);
			var rankedDiscsAdvises = await rankedDiscsAdviser.Advise(adviseGroups, playbacksInfo, cancellationToken);

			var playlistQueue = new CompositeAdvisedPlaylistQueue(highlyRatedSongsAdvises, favoriteArtistDiscsAdvises, rankedDiscsAdvises);

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

			if (playbacksMemo.PlaybacksSinceFavoriteArtistDisc + 1 >= settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs &&
			    playlistQueue.TryDequeueFavoriteArtistDiscsAdvise(out currentAdvise))
			{
				return currentAdvise;
			}

			return playlistQueue.TryDequeueRankedDiscsAdvise(out currentAdvise) ? currentAdvise : null;
		}

		private PlaylistAdviserMemo CreateDefaultMemo()
		{
			// If no previous PlaylistAdviserMemo exist, we're initializing memo with threshold values so that promoted advises go first.
			return new(settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs, settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs);
		}

		public async Task RegisterAdvicePlayback(AdvisedPlaylist advise, CancellationToken cancellationToken)
		{
			Memo = Memo.RegisterPlayback(advise);
			await sessionDataService.SaveData(PlaylistAdviserDataKey, Memo, cancellationToken);
		}
	}
}
