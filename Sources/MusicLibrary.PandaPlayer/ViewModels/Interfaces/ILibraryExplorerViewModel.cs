using System.Collections.ObjectModel;
using System.ComponentModel;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		FolderExplorerItem CurrentFolder { get; }

		ObservableCollection<FolderExplorerItem> Items { get; }

		FolderExplorerItem SelectedItem { get; set; }

		Disc SelectedDisc { get; }

		IExplorerSongListViewModel SongListViewModel { get; }

		void Load();

		void SwitchToDisc(Disc disc);
	}
}
