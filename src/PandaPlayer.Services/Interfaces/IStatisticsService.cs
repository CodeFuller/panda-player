using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IStatisticsService
	{
		Task<StatisticsModel> GetLibraryStatistics(CancellationToken cancellationToken);
	}
}
