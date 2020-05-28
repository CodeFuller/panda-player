using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class CompositePlaylistAdviser : ICompositePlaylistAdviser
	{
		private readonly IPlaylistAdviser usualDiscsAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favoriteArtistDiscsAdviser;
		private readonly IGenericDataRepository<PlaylistAdviserMemo> memoRepository;
		private readonly AdviserSettings settings;

		private readonly Lazy<PlaylistAdviserMemo> memo;

		public CompositePlaylistAdviser(IPlaylistAdviser usualDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser, IPlaylistAdviser favoriteArtistDiscsAdviser,
			IGenericDataRepository<PlaylistAdviserMemo> memoRepository, IOptions<AdviserSettings> options)
		{
			this.usualDiscsAdviser = usualDiscsAdviser ?? throw new ArgumentNullException(nameof(usualDiscsAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favoriteArtistDiscsAdviser = favoriteArtistDiscsAdviser ?? throw new ArgumentNullException(nameof(favoriteArtistDiscsAdviser));
			this.memoRepository = memoRepository ?? throw new ArgumentNullException(nameof(memoRepository));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			memo = new Lazy<PlaylistAdviserMemo>(() => memoRepository.Load() ??
			new PlaylistAdviserMemo
			{
				// Initializing memo with threshold values so that promoted advises go first.
				PlaybacksSinceHighlyRatedSongsPlaylist = settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs,
				PlaybacksSinceFavoriteArtistDisc = settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs,
			});
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs)
		{
			var discsList = discs.ToList();
			var playbacksMemo = (PlaylistAdviserMemo)memo.Value.Clone();

			var playbacksInfo = new PlaybacksInfo(discsList);

			var highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdviser.Advise(discsList, playbacksInfo));
			var favoriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favoriteArtistDiscsAdviser.Advise(discsList, playbacksInfo));
			var rankedDiscsAdvises = new Queue<AdvisedPlaylist>(usualDiscsAdviser.Advise(discsList, playbacksInfo));

			var advisedDiscs = new HashSet<ItemId>();
			while (rankedDiscsAdvises.Count > 0)
			{
				AdvisedPlaylist currAdvise;

				if (highlyRatedSongsAdvises.Count > 0 && playbacksMemo.PlaybacksSinceHighlyRatedSongsPlaylist + 1 >= settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs)
				{
					currAdvise = highlyRatedSongsAdvises.Dequeue();
				}
				else
				{
					var adviseQueue = favoriteArtistDiscsAdvises.Count > 0 &&
					                        playbacksMemo.PlaybacksSinceFavoriteArtistDisc + 1 >= settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs
						? favoriteArtistDiscsAdvises
						: rankedDiscsAdvises;

					currAdvise = adviseQueue.Dequeue();
					if (advisedDiscs.Contains(currAdvise.Disc.Id))
					{
						continue;
					}

					advisedDiscs.Add(currAdvise.Disc.Id);
				}

				// TODO: Move this playbacks memo logic to DiscAdviserViewModel
				playbacksMemo.RegisterPlayback(currAdvise);
				yield return currAdvise;
			}
		}

		public void RegisterAdvicePlayback(AdvisedPlaylist advise)
		{
			memo.Value.RegisterPlayback(advise);
			memoRepository.Save(memo.Value);
		}
	}
}
