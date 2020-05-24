using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	// TBD: Remove after redesign
	internal class FoldersRepository : IFoldersRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IDiscsRepository discsRepository;

		public FoldersRepository(IMusicLibraryDbContextFactory contextFactory, IDiscsRepository discsRepository)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
		}

		public Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			var rootId = new ItemId("/");
			return GetFolder(rootId, cancellationToken);
		}

		// TBD: Extend database with folder entity and remove this logic.
		public async Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var subfolders = new Dictionary<Uri, SubfolderModel>();
			var discIds = new List<ItemId>();

			foreach (var disc in context.Discs)
			{
				var childUri = GetDirectChildUri(folderId, disc.Uri);
				if (childUri == null)
				{
					continue;
				}

				if (childUri == disc.Uri)
				{
					discIds.Add(disc.Id.ToItemId());
				}
				else if (!subfolders.ContainsKey(childUri))
				{
					var subfolder = new SubfolderModel
					{
						Id = childUri.ToItemId(),
						Name = new ItemUriParts(childUri).Last(),
					};

					subfolders.Add(childUri, subfolder);
				}
			}

			var uriParts = new ItemUriParts(folderId.ToUri());
			var parentFolderId = uriParts.Any() ? ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId() : null;

			return new FolderModel
			{
				Id = folderId,
				ParentFolderId = parentFolderId,
				Subfolders = subfolders.Values,
				Discs = await discsRepository.GetDiscs(discIds, cancellationToken),
			};
		}

		private static Uri GetDirectChildUri(ItemId folderId, Uri childUri)
		{
			var parentParts = new ItemUriParts(folderId.ToUri());
			var childParts = new ItemUriParts(childUri);

			return parentParts.IsBaseOf(childParts) ? ItemUriParts.Join(childParts.Take(parentParts.Count + 1)) : null;
		}

		public async Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var id = discId.ToInt32();
			var disc = await context.Discs.SingleAsync(d => d.Id == id, cancellationToken);

			var uriParts = new ItemUriParts(disc.Uri);
			var parentFolderId = ItemUriParts.Join(uriParts.Take(uriParts.Count - 1)).ToItemId();

			return await GetFolder(parentFolderId, cancellationToken);
		}
	}
}
