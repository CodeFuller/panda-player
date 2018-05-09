using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class CompositePlaylistAdviser : ICompositePlaylistAdviser
	{
		private const int PlaybacksBetweenHighlyRatedSongsPlaylists = 10;

		private const int PlaybacksBetweenFavouriteArtistDiscs = 5;

		private readonly IPlaylistAdviser usualDiscsAdviser;
		private readonly IPlaylistAdviser highlyRatedSongsAdviser;
		private readonly IPlaylistAdviser favouriteArtistDiscsAdviser;
		private readonly IGenericDataRepository<PlaylistAdviserMemo> memoRepository;

		private readonly Lazy<PlaylistAdviserMemo> memo;

		public CompositePlaylistAdviser(IPlaylistAdviser usualDiscsAdviser, IPlaylistAdviser highlyRatedSongsAdviser, IPlaylistAdviser favouriteArtistDiscsAdviser,
			IGenericDataRepository<PlaylistAdviserMemo> memoRepository)
		{
			this.usualDiscsAdviser = usualDiscsAdviser ?? throw new ArgumentNullException(nameof(usualDiscsAdviser));
			this.highlyRatedSongsAdviser = highlyRatedSongsAdviser ?? throw new ArgumentNullException(nameof(highlyRatedSongsAdviser));
			this.favouriteArtistDiscsAdviser = favouriteArtistDiscsAdviser ?? throw new ArgumentNullException(nameof(favouriteArtistDiscsAdviser));
			this.memoRepository = memoRepository ?? throw new ArgumentNullException(nameof(memoRepository));

			memo = new Lazy<PlaylistAdviserMemo>(() => memoRepository.Load() ??
			new PlaylistAdviserMemo
			{
				//	Initializing memo with threshold values so that promoted advises go first.
				PlaybacksSinceHighlyRatedSongsPlaylist = PlaybacksBetweenHighlyRatedSongsPlaylists,
				PlaybacksSinceFavouriteArtistDisc = PlaybacksBetweenFavouriteArtistDiscs,
			});
		}

		public IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			PlaylistAdviserMemo playbacksMemo = (PlaylistAdviserMemo)memo.Value.Clone();

			var highlyRatedSongsAdvises = new Queue<AdvisedPlaylist>(highlyRatedSongsAdviser.Advise(discLibrary));
			var favouriteArtistDiscsAdvises = new Queue<AdvisedPlaylist>(favouriteArtistDiscsAdviser.Advise(discLibrary));
			var rankedDiscsAdvises = new Queue<AdvisedPlaylist>(usualDiscsAdviser.Advise(discLibrary));

			HashSet<Disc> advisedDiscs = new HashSet<Disc>();
			while (rankedDiscsAdvises.Count > 0)
			{
				AdvisedPlaylist currAdvise;

				if (highlyRatedSongsAdvises.Count > 0 && playbacksMemo.PlaybacksSinceHighlyRatedSongsPlaylist + 1 >= PlaybacksBetweenHighlyRatedSongsPlaylists)
				{
					currAdvise = highlyRatedSongsAdvises.Dequeue();
				}
				else
				{
					var adviseQueue = favouriteArtistDiscsAdvises.Count > 0 &&
					                        playbacksMemo.PlaybacksSinceFavouriteArtistDisc + 1 >= PlaybacksBetweenFavouriteArtistDiscs
						? favouriteArtistDiscsAdvises
						: rankedDiscsAdvises;

					currAdvise = adviseQueue.Dequeue();
					if (advisedDiscs.Contains(currAdvise.Disc))
					{
						continue;
					}
					advisedDiscs.Add(currAdvise.Disc);
				}

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
