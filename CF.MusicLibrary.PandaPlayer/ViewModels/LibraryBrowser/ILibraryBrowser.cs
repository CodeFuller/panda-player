using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public interface ILibraryBrowser
	{
		Task Load();

		IEnumerable<FolderExplorerItem> GetChildFolderItems(FolderExplorerItem folderItem);

		FolderExplorerItem GetParentFolder(FolderExplorerItem folderItem);
	}
}
