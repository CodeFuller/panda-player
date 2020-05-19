using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Services
{
	public interface IStatisticsService
	{
		Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken);
	}
}
