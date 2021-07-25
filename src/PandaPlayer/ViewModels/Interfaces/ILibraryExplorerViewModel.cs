using System.ComponentModel;
using System.Windows.Input;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		DiscModel SelectedDisc { get; }

		ICommand PlayDiscCommand { get; }

		ICommand AddDiscToPlaylistCommand { get; }

		ICommand EditDiscPropertiesCommand { get; }

		ICommand DeleteFolderCommand { get; }

		ICommand DeleteDiscCommand { get; }
	}
}
