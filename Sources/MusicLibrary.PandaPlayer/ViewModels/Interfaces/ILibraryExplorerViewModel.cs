using System.Collections.ObjectModel;
using System.ComponentModel;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ObservableCollection<BasicExplorerItem> Items { get; }

		BasicExplorerItem SelectedItem { get; set; }

		DiscModel SelectedDisc { get; }

		IExplorerSongListViewModel SongListViewModel { get; }

		void SwitchToDisc(DiscModel disc);
	}
}
