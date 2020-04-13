using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public interface ILibraryBrowser
	{
		IEnumerable<FolderExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem);

		FolderExplorerItem GetParentFolder(FolderExplorerItem folderItem);

		DiscExplorerItem GetDiscItem(Disc disc);
	}
}
