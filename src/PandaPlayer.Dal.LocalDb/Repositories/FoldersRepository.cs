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

		private readonly IContentUriProvider contentUriProvider;

		private readonly IFolderCache folderCache;

		public FoldersRepository(IDbContextFactory<MusicDbContext> contextFactory, IContentUriProvider contentUriProvider, IFolderCache folderCache)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.contentUriProvider = contentUriProvider ?? throw new ArgumentNullException(nameof(contentUriProvider));
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

		public async Task<IReadOnlyCollection<ShallowFolderModel>> GetAllFolders(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var folders = await GetShallowFoldersQueryable(context)
				.ToListAsync(cancellationToken);

			return folders.Select(f => f.ToShallowModel()).ToList();
		}

		public async Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var folder = await GetFoldersQueryable(context)
				.SingleAsync(f => f.ParentFolder == null, cancellationToken);

			return folder.ToModel(contentUriProvider);
		}

		public async Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var folder = await FindFolder(context, folderId, cancellationToken);

			return folder.ToModel(contentUriProvider);
		}

		public async Task UpdateFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var folderEntity = await FindShallowFolder(context, folder.Id, cancellationToken);

			var updatedEntity = folder.ToEntity();
			context.Entry(folderEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		private static async Task<FolderEntity> FindShallowFolder(MusicDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await GetShallowFoldersQueryable(context)
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}

		private static async Task<FolderEntity> FindFolder(MusicDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await GetFoldersQueryable(context)
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}

		private static IQueryable<FolderEntity> GetShallowFoldersQueryable(MusicDbContext context)
		{
			return context.Folders
				.Include(f => f.ParentFolder).ThenInclude(f => f.AdviseGroup)
				.Include(f => f.AdviseGroup);
		}

		private static IQueryable<FolderEntity> GetFoldersQueryable(MusicDbContext context)
		{
			return context.Folders
				.Include(f => f.ParentFolder).ThenInclude(f => f.AdviseGroup)
				.Include(f => f.AdviseGroup)
				.Include(f => f.Subfolders).ThenInclude(f => f.AdviseGroup)
				.Include(f => f.Discs).ThenInclude(d => d.AdviseGroup)
				.Include(f => f.Discs).ThenInclude(d => d.Songs).ThenInclude(s => s.Artist)
				.Include(f => f.Discs).ThenInclude(d => d.Songs).ThenInclude(s => s.Genre)
				.Include(f => f.Discs).ThenInclude(d => d.Images);
		}
	}
}
