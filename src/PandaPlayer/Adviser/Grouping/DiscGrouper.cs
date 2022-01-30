using System;
using System.Collections.Generic;
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

		public Task<IReadOnlyCollection<AdviseGroupContent>> GroupLibraryDiscs(IEnumerable<DiscModel> discs, CancellationToken cancellationToken)
		{
			var adviseGroups = new Dictionary<string, AdviseGroupContent>();

			var folderAdviseGroupCache = new Dictionary<ItemId, AdviseGroupModel>();

			// For each disc we search for closest parent folder with assigned advise group.
			// If there are no assigned advise group up to the root, then own parent folder is used as implicit group.
			foreach (var disc in discs)
			{
				var adviseGroup = disc.AdviseGroup ?? GetFolderAdviseGroup(disc.Folder, folderAdviseGroupCache);

				var groupId = adviseGroup != null ? $"Advise Group: {adviseGroup.Id}" : $"Folder Group: {disc.Folder.Id}";

				if (!adviseGroups.TryGetValue(groupId, out var adviseGroupContent))
				{
					adviseGroupContent = new AdviseGroupContent(groupId, isFavorite: adviseGroup?.IsFavorite ?? false);
					adviseGroups.Add(groupId, adviseGroupContent);
				}

				adviseGroupContent.AddDisc(disc);
			}

			return Task.FromResult<IReadOnlyCollection<AdviseGroupContent>>(adviseGroups.Values);
		}

		private static AdviseGroupModel GetFolderAdviseGroup(FolderModel folder, IDictionary<ItemId, AdviseGroupModel> folderAdviseGroupCache)
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

			var adviseGroup = GetFolderAdviseGroup(folder.ParentFolder, folderAdviseGroupCache);
			folderAdviseGroupCache.Add(folder.Id, adviseGroup);

			return adviseGroup;
		}
	}
}
