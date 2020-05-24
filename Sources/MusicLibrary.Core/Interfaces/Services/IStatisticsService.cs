using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Services
{
	public interface IStatisticsService
	{
		Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken);
	}
}
