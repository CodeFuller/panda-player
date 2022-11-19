using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryExplorerViewModel : INotifyPropertyChanged
	{
		ILibraryExplorerItemListViewModel ItemListViewModel { get; }

		DiscModel SelectedDisc { get; }

		IEnumerable<BasicMenuItem> ContextMenuItemsForSelectedItem { get; }

		Task CreateAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, CancellationToken cancellationToken);

		void PlayDisc(DiscModel disc);

		void AddDiscToPlaylist(DiscModel disc);

		void EditDiscProperties(DiscModel disc);

		void RenameFolder(FolderModel folder);

		Task DeleteFolder(FolderModel folder, CancellationToken cancellationToken);

		Task DeleteDisc(DiscModel disc, CancellationToken cancellationToken);
	}
}
