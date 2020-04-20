using System.Collections.ObjectModel;
using System.ComponentModel;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ObservableCollection<BasicExplorerItem> Items { get; }

		BasicExplorerItem SelectedItem { get; set; }

		Disc SelectedDisc { get; }

		IExplorerSongListViewModel SongListViewModel { get; }

		void SwitchToDisc(Disc disc);
	}
}
