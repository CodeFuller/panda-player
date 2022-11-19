using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class LibraryStatisticsViewModel : ViewModelBase, ILibraryStatisticsViewModel
	{
		private readonly IStatisticsService statisticsService;

		private StatisticsModel Statistics { get; set; }

		public int ArtistsNumber => Statistics.ArtistsNumber;

		public int DiscArtistsNumber => Statistics.DiscArtistsNumber;

		public int DiscsNumber => Statistics.DiscsNumber;

		public int SongsNumber => Statistics.SongsNumber;

		public long StorageSize => Statistics.StorageSize;

		public TimeSpan SongsDuration => Statistics.SongsDuration;

		public TimeSpan PlaybacksDuration => Statistics.PlaybacksDuration;

		public int PlaybacksNumber => Statistics.PlaybacksNumber;

		public double UnheardSongsPercentage => CalculatePercentage(Statistics.UnheardSongsNumber, SongsNumber);

		public double UnratedSongsPercentage => CalculatePercentage(Statistics.UnratedSongsNumber, SongsNumber);

		public double PercentageOfDiscsWithoutCoverImage => CalculatePercentage(Statistics.NumberOfDiscsWithoutCoverImage, DiscsNumber);

		public LibraryStatisticsViewModel(IStatisticsService statisticsService)
		{
			this.statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			Statistics = await statisticsService.GetLibraryStatistics(cancellationToken);
		}

		private static double CalculatePercentage(int value, int totalCount)
		{
			return totalCount > 0 ? (double)value / totalCount : 0;
		}
	}
}
