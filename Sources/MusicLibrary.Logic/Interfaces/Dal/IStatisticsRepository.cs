using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IStatisticsRepository
	{
		Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken);
	}
}
