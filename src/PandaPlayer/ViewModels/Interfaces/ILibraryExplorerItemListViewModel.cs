using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

		FolderModel SelectedFolder { get; }

		DiscModel SelectedDisc { get; }

		ICommand ChangeFolderCommand { get; }

		ICommand JumpToFirstItemCommand { get; }

		ICommand JumpToLastItemCommand { get; }

		void LoadFolderItems(FolderModel folder);

		void SelectFolder(ItemId folderId);

		void SelectDisc(ItemId discId);

		Task OnFolderDeleted(ItemId folderId);

		Task OnDiscDeleted(ItemId discId);
	}
}
