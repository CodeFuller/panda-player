using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
{
	internal class StatisticsService : IStatisticsService
	{
		private readonly IStatisticsRepository statisticsRepository;

		public StatisticsService(IStatisticsRepository statisticsRepository)
		{
			this.statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));
		}

		public Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken)
		{
			return statisticsRepository.GetLibraryStatistics(cancellationToken);
		}
	}
}
