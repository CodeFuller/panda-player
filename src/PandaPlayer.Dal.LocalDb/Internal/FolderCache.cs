using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Interfaces;

namespace PandaPlayer.Dal.LocalDb.Internal
{
	internal class FolderCache : IFolderCache, IFolderProvider
	{
		private readonly ConcurrentDictionary<ItemId, ShallowFolderModel> folders = new();

		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public FolderCache(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public void StoreFolder(ShallowFolderModel folder)
		{
			folders.AddOrUpdate(folder.Id, folder, (k, v) => folder);
		}

		public void Clear()
		{
			folders.Clear();
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
				.Include(x => x.AdviseGroup)
				.Select(f => f.ToShallowModel());

			foreach (var folder in allFolders)
			{
				StoreFolder(folder);
			}
		}
	}
}
