using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class LibraryStatisticsViewModel : ViewModelBase, ILibraryStatisticsViewModel
	{
		private readonly DiscLibrary discLibrary;

		public int ArtistsNumber => discLibrary.Artists.Count();

		public int DiscArtistsNumber => discLibrary.Discs.Select(d => d.Artist).Where(a => a != null).Distinct().Count();

		public int DiscsNumber => discLibrary.Discs.Count();

		public int SongsNumber => discLibrary.Songs.Count();

		public long StorageSize => discLibrary.Songs.Sum(s => (long)s.FileSize);

		public TimeSpan TotalDuration => discLibrary.Songs.Aggregate(TimeSpan.Zero, (currSum, currSong) => currSum + currSong.Duration);

		public TimeSpan ListensDuration => TimeSpan.FromTicks(discLibrary.AllSongs.Sum(song => song.PlaybacksCount * song.Duration.Ticks));

		public int ListensNumber => discLibrary.AllSongs.Sum(song => song.PlaybacksCount);

		private int UnlistenedSongsNumber => discLibrary.Songs.Count(s => s.PlaybacksCount == 0);

		public double UnlistenedSongsPercentage
		{
			get
			{
				var totalSongsNumber = SongsNumber;
				return totalSongsNumber > 0 ? (double)UnlistenedSongsNumber / totalSongsNumber : 0;
			}
		}

		private int UnratedSongsNumber => discLibrary.Songs.Count(s => s.Rating == null);

		public double UnratedSongsPercentage
		{
			get
			{
				var totalSongsNumber = SongsNumber;
				return totalSongsNumber > 0 ? (double)UnratedSongsNumber / totalSongsNumber : 0;
			}
		}

		private int NumberOfDiscsWithoutCoverImage => discLibrary.Discs.Count(disc => disc.CoverImage == null);

		public double PercentageOfDiscsWithoutCoverImage
		{
			get
			{
				var totalDiscsNumber = DiscsNumber;
				return totalDiscsNumber > 0 ? (double)NumberOfDiscsWithoutCoverImage / totalDiscsNumber : 0;
			}
		}

		public LibraryStatisticsViewModel(DiscLibrary discLibrary)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}

			this.discLibrary = discLibrary;
		}
	}
}
