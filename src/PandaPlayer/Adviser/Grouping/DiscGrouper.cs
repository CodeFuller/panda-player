using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Adviser.Grouping
{
	internal class DiscGrouper : IDiscGrouper
	{
		private readonly IFoldersService foldersService;

		public DiscGrouper(IFoldersService foldersService)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
		}

		public async Task<IReadOnlyCollection<AdviseGroupContent>> GroupLibraryDiscs(IEnumerable<DiscModel> discs, CancellationToken cancellationToken)
		{
			var groups = new Dictionary<string, AdviseGroupContent>();

			var allFolders = (await foldersService.GetAllFolders(cancellationToken))
				.ToDictionary(x => x.Id, x => x);

			var folderAdviseGroupCache = new Dictionary<ItemId, AdviseGroupModel>();

			// For each disc we search for closest parent folder with assigned advise group.
			// If there are no assigned advise group up to the root, then own parent folder is used as implicit group.
			foreach (var disc in discs)
			{
				var adviseGroup = disc.AdviseGroup ?? GetFolderAdviseGroup(disc.Folder, folderAdviseGroupCache, allFolders);

				var groupId = adviseGroup != null ? $"Advise Group: {adviseGroup.Id}" : $"Folder Group: {disc.Folder.Id}";

				if (!groups.TryGetValue(groupId, out var group))
				{
					group = new AdviseGroupContent(groupId);
					groups.Add(groupId, group);
				}

				group.AddDisc(disc);
			}

			return groups.Values;
		}

		private static AdviseGroupModel GetFolderAdviseGroup(ShallowFolderModel folder, IDictionary<ItemId, AdviseGroupModel> folderAdviseGroupCache, IReadOnlyDictionary<ItemId, ShallowFolderModel> allFolders)
		{
			if (folder.AdviseGroup != null)
			{
				return folder.AdviseGroup;
			}

			if (folder.IsRoot)
			{
				return null;
			}

			if (folderAdviseGroupCache.TryGetValue(folder.Id, out var cachedAdviseGroup))
			{
				return cachedAdviseGroup;
			}

			if (!allFolders.TryGetValue(folder.ParentFolderId, out var parentFolder))
			{
				throw new InvalidOperationException($"The parent folder for id {folder.ParentFolderId} is missing");
			}

			var adviseGroup = GetFolderAdviseGroup(parentFolder, folderAdviseGroupCache, allFolders);

			folderAdviseGroupCache.Add(folder.Id, adviseGroup);

			return adviseGroup;
		}
	}
}
