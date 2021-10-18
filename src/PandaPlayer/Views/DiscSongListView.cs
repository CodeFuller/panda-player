using System.Windows;
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
		public DiscSongListView()
		{
			InitializeComponent();

			SongsDataGrid.ContextMenuOpening += DataGrid_ContextMenuOpening;
		}

		private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (e.Source is not FrameworkElement frameworkElement || DataContext is not IDiscSongListViewModel viewModel)
			{
				return;
			}

			frameworkElement.ContextMenu = viewModel.ContextMenuItems.ToContextMenu();
		}
	}
}
