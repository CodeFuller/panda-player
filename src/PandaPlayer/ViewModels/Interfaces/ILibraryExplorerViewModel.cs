using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
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

		Task SwitchToDisc(DiscModel disc, CancellationToken cancellationToken);
	}
}
