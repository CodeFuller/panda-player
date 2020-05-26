using System;
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
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IContentUriProvider contentUriProvider;

		public FoldersRepository(IMusicLibraryDbContextFactory contextFactory, IContentUriProvider contentUriProvider)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.contentUriProvider = contentUriProvider ?? throw new ArgumentNullException(nameof(contentUriProvider));
		}

		public async Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var folder = await GetFoldersQueryable(context)
				.SingleAsync(f => f.ParentFolder == null, cancellationToken);

			return folder.ToModel(contentUriProvider);
		}

		public async Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var id = folderId.ToInt32();
			var folder = await GetFoldersQueryable(context)
				.SingleAsync(f => f.Id == id, cancellationToken);

			return folder.ToModel(contentUriProvider);
		}

		public async Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var id = discId.ToInt32();
			var folder = await GetFoldersQueryable(context)
				.SingleAsync(f => f.Discs.Any(d => d.Id == id), cancellationToken);

			return folder.ToModel(contentUriProvider);
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
