using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class FolderCache : IFolderCache, IFolderProvider
	{
		private readonly ConcurrentDictionary<ItemId, ShallowFolderModel> folders = new ConcurrentDictionary<ItemId, ShallowFolderModel>();

		private readonly IDbContextFactory<MusicLibraryDbContext> contextFactory;

		public FolderCache(IDbContextFactory<MusicLibraryDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public void StoreFolder(ShallowFolderModel folder)
		{
			folders.AddOrUpdate(folder.Id, folder, (k, v) => folder);
		}

		public ShallowFolderModel GetFolder(ItemId folderId)
		{
			if (!folders.Any())
			{
				InitializeCache();
			}

			if (folders.TryGetValue(folderId, out var folder))
			{
				return folder;
			}

			throw new InvalidOperationException($"The folder {folderId} is missing in the cache");
		}

		private void InitializeCache()
		{
			var context = contextFactory.CreateDbContext();

			var allFolders = context.Folders
				.Select(f => f.ToShallowModel());

			foreach (var folder in allFolders)
			{
				StoreFolder(folder);
			}
		}
	}
}
