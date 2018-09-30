using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public interface ILibraryBrowser
	{
		IEnumerable<FolderExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem);

		FolderExplorerItem GetParentFolder(FolderExplorerItem folderItem);

		DiscExplorerItem GetDiscItem(Disc disc);
	}
}
