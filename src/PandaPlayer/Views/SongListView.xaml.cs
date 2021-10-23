using System.Windows.Controls;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	public partial class SongListView : UserControl
	{
		private ISongListViewModel ViewModel => DataContext.GetViewModel<ISongListViewModel>();

		public SongListView()
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
