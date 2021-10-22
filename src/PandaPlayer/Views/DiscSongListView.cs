using System.Windows.Controls;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	// It is not possible to have XAML for UserControl which inherits from another UserControl.
	// That's why we define this control only via code-behind.
	// https://stackoverflow.com/questions/269106/
	public class DiscSongListView : SongListView
	{
		private IDiscSongListViewModel ViewModel => DataContext.GetViewModel<IDiscSongListViewModel>();

		public DiscSongListView()
		{
			InitializeComponent();

			// Without this, context menu will not open first time.
			SongsDataGrid.ContextMenu = new ContextMenu();

			SongsDataGrid.ContextMenuOpening += DataGrid_ContextMenuOpening;
		}

		private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			SongsDataGrid.ContextMenu = ViewModel.ContextMenuItems.ToContextMenu();
		}
	}
}
