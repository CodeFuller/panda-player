using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerItemListViewModel
	{
		ObservableCollection<BasicExplorerItem> Items { get; }

		bool ShowDeletedContent { get; set; }

		BasicExplorerItem SelectedItem { get; set; }

		IEnumerable<DiscModel> Discs { get; }

		ShallowFolderModel SelectedFolder { get; }

		DiscModel SelectedDisc { get; }

		ICommand ChangeFolderCommand { get; }

		ICommand JumpToFirstItemCommand { get; }

		ICommand JumpToLastItemCommand { get; }

		void LoadFolderItems(FolderModel folder);

		void SelectFolder(ItemId folderId);

		void SelectDisc(ItemId discId);

		void RemoveFolder(ItemId folderId);

		void RemoveDisc(ItemId discId);
	}
}
