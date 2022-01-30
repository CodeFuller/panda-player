using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class FoldersRepository : IFoldersRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public FoldersRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateEmptyFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			var folderEntity = folder.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.Folders.AddAsync(folderEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			folder.Id = folderEntity.Id.ToItemId();
		}

		public async Task UpdateFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var folderEntity = await FindFolder(context, folder.Id, cancellationToken);

			var updatedEntity = folder.ToEntity();
			context.Entry(folderEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		private static async Task<FolderEntity> FindFolder(MusicDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();

			return await context.Folders
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}
	}
}
