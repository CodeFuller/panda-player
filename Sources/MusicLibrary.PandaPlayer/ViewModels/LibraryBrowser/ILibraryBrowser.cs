using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public interface ILibraryBrowser
	{
		IEnumerable<LibraryExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem);

		FolderExplorerItem GetParentFolder(LibraryExplorerItem item);

		DiscExplorerItem GetDiscItem(Disc disc);
	}
}
