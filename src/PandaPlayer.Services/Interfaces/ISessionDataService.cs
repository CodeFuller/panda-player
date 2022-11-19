using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.Services.Interfaces
{
	public interface ISessionDataService
	{
		Task SaveData<TData>(string key, TData data, CancellationToken cancellationToken);

		Task<TData> GetData<TData>(string key, CancellationToken cancellationToken)
			where TData : class;

		Task PurgeData(string key, CancellationToken cancellationToken);
	}
}
