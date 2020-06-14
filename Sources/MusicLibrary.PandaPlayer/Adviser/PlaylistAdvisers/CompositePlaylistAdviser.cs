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

		private PlaylistAdviserMemo Memo { get; set; }

		public CompositePlaylistAdviser(IPlaylistAdviser usualDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser, IPlaylistAdviser favoriteArtistDiscsAdviser,
			IGenericDataRepository<PlaylistAdviserMemo> memoRepository, IOptions<AdviserSettings> options)
		{
			this.usualDiscsAdviser = usualDiscsAdviser ?? throw new ArgumentNullException(nameof(usualDiscsAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favoriteArtistDiscsAdviser = favoriteArtistDiscsAdviser ?? throw new ArgumentNullException(nameof(favoriteArtistDiscsAdviser));
			this.memoRepository = memoRepository ?? throw new ArgumentNullException(nameof(memoRepository));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs)
		{
			Memo ??= memoRepository.Load() ?? CreateDefaultMemo();
			var playbacksMemo = Memo;

			var discsList = discs.ToList();
			var playbacksInfo = new PlaybacksInfo(discsList);

			var highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdviser.Advise(discsList, playbacksInfo));
			var favoriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favoriteArtistDiscsAdviser.Advise(discsList, playbacksInfo));
			var rankedDiscsAdvises = new Queue<AdvisedPlaylist>(usualDiscsAdviser.Advise(discsList, playbacksInfo));

			var advisedDiscs = new HashSet<ItemId>();
			while (rankedDiscsAdvises.Count > 0)
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

				playbacksMemo = playbacksMemo.RegisterPlayback(currentAdvise);
				yield return currentAdvise;
			}
		}

		private PlaylistAdviserMemo CreateDefaultMemo()
		{
			// If no previous PlaylistAdviserMemo exist, we're initializing memo with threshold values so that promoted advises go first.
			return new PlaylistAdviserMemo(settings.HighlyRatedSongsAdviser.PlaybacksBetweenHighlyRatedSongs, settings.FavoriteArtistsAdviser.PlaybacksBetweenFavoriteArtistDiscs);
		}

		public void RegisterAdvicePlayback(AdvisedPlaylist advise)
		{
			Memo = Memo.RegisterPlayback(advise);
			memoRepository.Save(Memo);
		}
	}
}
