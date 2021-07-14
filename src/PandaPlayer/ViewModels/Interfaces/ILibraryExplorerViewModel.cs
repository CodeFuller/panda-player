using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ObservableCollection<BasicExplorerItem> Items { get; }

		BasicExplorerItem SelectedItem { get; set; }

		DiscModel SelectedDisc { get; }

		IDiscSongListViewModel DiscSongListViewModel { get; }

		ICommand ChangeFolderCommand { get; }

		ICommand PlayDiscCommand { get; }

		ICommand AddDiscToPlaylistCommand { get; }

		ICommand DeleteDiscCommand { get; }

		ICommand JumpToFirstItemCommand { get; }

		ICommand JumpToLastItemCommand { get; }

		ICommand EditDiscPropertiesCommand { get; }

		ICommand DeleteFolderCommand { get; }

		Task SwitchToDisc(DiscModel disc, CancellationToken cancellationToken);
	}
}
