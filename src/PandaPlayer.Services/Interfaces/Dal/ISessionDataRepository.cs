using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface ISessionDataRepository
	{
		Task SaveData<TData>(string key, TData data, CancellationToken cancellationToken);

		Task<TData> GetData<TData>(string key, CancellationToken cancellationToken)
			where TData : class;

		Task DeleteData(string key, CancellationToken cancellationToken);
	}
}
