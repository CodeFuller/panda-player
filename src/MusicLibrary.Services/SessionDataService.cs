using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
{
	internal class SessionDataService : ISessionDataService
	{
		private readonly ISessionDataRepository repository;

		public SessionDataService(ISessionDataRepository repository)
		{
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		public Task SaveData<TData>(string key, TData data, CancellationToken cancellationToken)
		{
			return repository.SaveData(key, data, cancellationToken);
		}

		public Task<TData> GetData<TData>(string key, CancellationToken cancellationToken)
			where TData : class
		{
			return repository.GetData<TData>(key, cancellationToken);
		}

		public Task PurgeData(string key, CancellationToken cancellationToken)
		{
			return repository.DeleteData(key, cancellationToken);
		}
	}
}
