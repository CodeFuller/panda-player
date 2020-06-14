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

		private readonly IPlaylistAdviser usualDiscsAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favoriteArtistDiscsAdviser;
		private readonly ISessionDataService sessionDataService;
		private readonly AdviserSettings settings;

		private PlaylistAdviserMemo Memo { get; set; }

		public CompositePlaylistAdviser(IPlaylistAdviser usualDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser,
			IPlaylistAdviser favoriteArtistDiscsAdviser, ISessionDataService sessionDataService, IOptions<AdviserSettings> options)
		{
			this.usualDiscsAdviser = usualDiscsAdviser ?? throw new ArgumentNullException(nameof(usualDiscsAdviser));
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

			var highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdviser.Advise(discsList, playbacksInfo));
			var favoriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favoriteArtistDiscsAdviser.Advise(discsList, playbacksInfo));
			var rankedDiscsAdvises = new Queue<AdvisedPlaylist>(usualDiscsAdviser.Advise(discsList, playbacksInfo));

			var advisedPlaylists = new List<AdvisedPlaylist>(requiredAdvisesCount);

			var advisedDiscs = new HashSet<ItemId>();
			while (rankedDiscsAdvises.Count > 0 && advisedPlaylists.Count < requiredAdvisesCount)
			{
				AdvisedPlaylist currentAdvise;

				if (highlyRatedSongsAdvises.Count > 0 && playbacksMemo.PlaybacksSinceHighlyRatedSongsPlaylist + 1 >= settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs)
				{
					currentAdvise = highlyRatedSongsAdvises.Dequeue();
				}
				else
				{
					var adviseQueue = favoriteArtistDiscsAdvises.Count > 0 &&
					                        playbacksMemo.PlaybacksSinceFavoriteArtistDisc + 1 >= settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs
						? favoriteArtistDiscsAdvises
						: rankedDiscsAdvises;

					currentAdvise = adviseQueue.Dequeue();
					if (advisedDiscs.Contains(currentAdvise.Disc.Id))
					{
						continue;
					}

					advisedDiscs.Add(currentAdvise.Disc.Id);
				}

				advisedPlaylists.Add(currentAdvise);
				playbacksMemo = playbacksMemo.RegisterPlayback(currentAdvise);
			}

			return advisedPlaylists;
		}

		private PlaylistAdviserMemo CreateDefaultMemo()
		{
			// If no previous PlaylistAdviserMemo exist, we're initializing memo with threshold values so that promoted advises go first.
			return new PlaylistAdviserMemo(settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs, settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs);
		}

		public async Task RegisterAdvicePlayback(AdvisedPlaylist advise, CancellationToken cancellationToken)
		{
			Memo = Memo.RegisterPlayback(advise);
			await sessionDataService.SaveData(PlaylistAdviserDataKey, Memo, cancellationToken);
		}
	}
}
