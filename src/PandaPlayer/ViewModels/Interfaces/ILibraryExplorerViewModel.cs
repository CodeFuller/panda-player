using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		DiscModel SelectedDisc { get; }

		IReadOnlyCollection<BasicMenuItem> AdviseGroupMenuItems { get; }

		ICommand PlayDiscCommand { get; }

		ICommand AddDiscToPlaylistCommand { get; }

		ICommand EditDiscPropertiesCommand { get; }

		ICommand DeleteFolderCommand { get; }

		ICommand DeleteDiscCommand { get; }
	}
}
