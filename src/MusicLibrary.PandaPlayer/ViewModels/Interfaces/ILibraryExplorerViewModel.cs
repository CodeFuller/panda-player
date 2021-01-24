using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.LibraryExplorerItems;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ObservableCollection<BasicExplorerItem> Items { get; }

		BasicExplorerItem SelectedItem { get; set; }

		DiscModel SelectedDisc { get; }

		IExplorerSongListViewModel SongListViewModel { get; }

		Task SwitchToDisc(DiscModel disc, CancellationToken cancellationToken);
	}
}
