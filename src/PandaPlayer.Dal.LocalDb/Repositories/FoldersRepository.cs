using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class FoldersRepository : IFoldersRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		private readonly IFolderCache folderCache;

		public FoldersRepository(IDbContextFactory<MusicDbContext> contextFactory, IFolderCache folderCache)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.folderCache = folderCache ?? throw new ArgumentNullException(nameof(folderCache));
		}

		public async Task CreateFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			var folderEntity = folder.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.Folders.AddAsync(folderEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			folder.Id = folderEntity.Id.ToItemId();

			folderCache.StoreFolder(folder);
		}

		public async Task<IReadOnlyCollection<ShallowFolderModel>> GetFoldersWithoutDiscs(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var folders = await context.Folders
				.Include(x => x.Discs)
				.Include(f => f.ParentFolder).ThenInclude(f => f.AdviseGroup)
				.Include(f => f.AdviseGroup)
				.Where(x => !x.Discs.Any())
				.ToListAsync(cancellationToken);

			return folders.Select(f => f.ToShallowModel()).ToList();
		}

		public async Task UpdateFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var folderEntity = await FindShallowFolder(context, folder.Id, cancellationToken);

			var updatedEntity = folder.ToEntity();
			context.Entry(folderEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);

			folderCache.Clear();
		}

		private static async Task<FolderEntity> FindShallowFolder(MusicDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();

			return await context.Folders
				.Include(f => f.ParentFolder).ThenInclude(f => f.AdviseGroup)
				.Include(f => f.AdviseGroup)
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}
	}
}
