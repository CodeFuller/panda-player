using System.Collections.Generic;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public interface ILibraryBrowser
	{
		IEnumerable<FolderExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem);

		FolderExplorerItem GetParentFolder(FolderExplorerItem folderItem);

		void RemoveDiscItem(DiscExplorerItem discItem);
	}
}
