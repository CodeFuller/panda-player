using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IStatisticsService
	{
		Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken);
	}
}
