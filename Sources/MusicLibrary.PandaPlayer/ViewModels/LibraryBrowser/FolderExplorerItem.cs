using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		private readonly SubfolderModel folder;

		public ItemId FolderId => folder.Id;

		public override string Title => folder.Name;

		public FolderExplorerItem(SubfolderModel folder)
		{
			this.folder = folder ?? throw new ArgumentNullException(nameof(folder));
		}
	}
}
