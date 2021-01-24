using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;
using Newtonsoft.Json;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class SessionDataRepository : ISessionDataRepository
	{
		private readonly IDbContextFactory<MusicLibraryDbContext> contextFactory;

		public SessionDataRepository(IDbContextFactory<MusicLibraryDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task SaveData<TData>(string key, TData data, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var dataEntity = await FindSessionData(context, key, cancellationToken);

			var serializedData = SerializeData(data);

			if (dataEntity != null)
			{
				dataEntity.Data = serializedData;
				context.Update(dataEntity);
			}
			else
			{
				dataEntity = new SessionDataEntity
				{
					Key = key,
					Data = serializedData,
				};

				await context.AddAsync(dataEntity, cancellationToken);
			}

			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task<TData> GetData<TData>(string key, CancellationToken cancellationToken)
			where TData : class
		{
			await using var context = contextFactory.CreateDbContext();
			var dataEntity = await FindSessionData(context, key, cancellationToken);

			return DeserializeData<TData>(dataEntity?.Data);
		}

		public async Task DeleteData(string key, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var dataEntity = await context.SessionData.SingleOrDefaultAsync(sd => sd.Key == key, cancellationToken);

			if (dataEntity == null)
			{
				return;
			}

			// We just clear the data property, without deleting the entity from the table.
			// This is done to avoid numerous DELETE/INSERT calls that could cause fragmentation issues.
			dataEntity.Data = null;
			await context.SaveChangesAsync(cancellationToken);
		}

		private static Task<SessionDataEntity> FindSessionData(MusicLibraryDbContext context, string key, CancellationToken cancellationToken)
		{
			return context.SessionData.SingleOrDefaultAsync(sd => sd.Key == key, cancellationToken);
		}

		private static string SerializeData<TData>(TData data)
		{
			return data == null ? null : JsonConvert.SerializeObject(data, Formatting.Indented);
		}

		private static TData DeserializeData<TData>(string data)
			where TData : class
		{
			return data == null ? null : JsonConvert.DeserializeObject<TData>(data);
		}
	}
}
