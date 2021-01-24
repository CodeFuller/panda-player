using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class FoldersRepository : IFoldersRepository
	{
		private readonly IDbContextFactory<MusicLibraryDbContext> contextFactory;

		private readonly IContentUriProvider contentUriProvider;

		private readonly IFolderCache folderCache;

		public FoldersRepository(IDbContextFactory<MusicLibraryDbContext> contextFactory, IContentUriProvider contentUriProvider, IFolderCache folderCache)
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

		private static async Task<FolderEntity> FindShallowFolder(MusicLibraryDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await GetShallowFoldersQueryable(context)
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}

		private static async Task<FolderEntity> FindFolder(MusicLibraryDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await GetFoldersQueryable(context)
				.SingleAsync(f => f.Id == entityId, cancellationToken);
		}

		private static IQueryable<FolderEntity> GetShallowFoldersQueryable(MusicLibraryDbContext context)
		{
			return context.Folders
				.Include(f => f.ParentFolder);
		}

		private static IQueryable<FolderEntity> GetFoldersQueryable(MusicLibraryDbContext context)
		{
			return context.Folders
				.Include(f => f.ParentFolder)
				.Include(f => f.Subfolders)
				.Include(f => f.Discs).ThenInclude(d => d.Songs).ThenInclude(s => s.Artist)
				.Include(f => f.Discs).ThenInclude(d => d.Songs).ThenInclude(s => s.Genre)
				.Include(f => f.Discs).ThenInclude(d => d.Images);
		}
	}
}
