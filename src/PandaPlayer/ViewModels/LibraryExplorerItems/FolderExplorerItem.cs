using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.LibraryExplorerItems
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		private readonly ShallowFolderModel folder;

		public ItemId FolderId => folder.Id;

		public override string Title => folder.Name;

		public FolderExplorerItem(ShallowFolderModel folder)
		{
			this.folder = folder ?? throw new ArgumentNullException(nameof(folder));
		}
	}
}
