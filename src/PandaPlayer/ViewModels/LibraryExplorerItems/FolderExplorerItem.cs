using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		public ItemId FolderId => Folder.Id;

		public ShallowFolderModel Folder { get; }

		public override string Title => Folder.Name;

		public FolderExplorerItem(ShallowFolderModel folder)
		{
			Folder = folder ?? throw new ArgumentNullException(nameof(folder));
		}
	}
}
