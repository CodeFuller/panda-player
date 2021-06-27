using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.Settings;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositePlaylistAdviser : ICompositePlaylistAdviser
	{
		private const string PlaylistAdviserDataKey = "PlaylistAdviserData";

		private readonly IPlaylistAdviser rankedDiscsAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favoriteArtistDiscsAdviser;
		private readonly ISessionDataService sessionDataService;
		private readonly AdviserSettings settings;

		private PlaylistAdviserMemo Memo { get; set; }

		public CompositePlaylistAdviser(IPlaylistAdviser rankedDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser,
			IPlaylistAdviser favoriteArtistDiscsAdviser, ISessionDataService sessionDataService, IOptions<AdviserSettings> options)
		{
			this.rankedDiscsAdviser = rankedDiscsAdviser ?? throw new ArgumentNullException(nameof(rankedDiscsAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favoriteArtistDiscsAdviser = favoriteArtistDiscsAdviser ?? throw new ArgumentNullException(nameof(favoriteArtistDiscsAdviser));
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, int requiredAdvisesCount, CancellationToken cancellationToken)
		{
			Memo ??= await sessionDataService.GetData<PlaylistAdviserMemo>(PlaylistAdviserDataKey, cancellationToken) ?? CreateDefaultMemo();
			var playbacksMemo = Memo;

			var discsList = discs.ToList();
			var playbacksInfo = new PlaybacksInfo(discsList);

			var highlyRatedSongsAdvises = highlyRatedSongsAdviser.Advise(discsList, playbacksInfo);
			var favoriteArtistDiscsAdvises = favoriteArtistDiscsAdviser.Advise(discsList, playbacksInfo);
			var rankedDiscsAdvises = rankedDiscsAdviser.Advise(discsList, playbacksInfo);

			var playlistQueue = new CompositeAdvisedPlaylistQueue(highlyRatedSongsAdvises, favoriteArtistDiscsAdvises, rankedDiscsAdvises);

			var advisedPlaylists = new List<AdvisedPlaylist>(requiredAdvisesCount);

			var advisedDiscs = new HashSet<ItemId>();
			while (advisedPlaylists.Count < requiredAdvisesCount)
			{
				var currentAdvise = GetNextAdvisedPlaylist(playbacksMemo, playlistQueue);
				if (currentAdvise == null)
				{
					break;
				}

				var advisedDisc = currentAdvise.Disc;
				if (advisedDisc != null)
				{
					if (advisedDiscs.Contains(advisedDisc.Id))
					{
						continue;
					}

					advisedDiscs.Add(advisedDisc.Id);
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
