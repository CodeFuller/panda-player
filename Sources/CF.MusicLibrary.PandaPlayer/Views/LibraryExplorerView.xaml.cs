using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for LibraryExplorerView.xaml.
	/// </summary>
	public partial class LibraryExplorerView : UserControl
	{
		public LibraryExplorerView()
		{
			InitializeComponent();
		}

		// https://stackoverflow.com/a/29081353/5740031
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dg = sender as DataGrid;
			if (dg == null || dg.SelectedIndex < 0)
			{
				return;
			}

			SelectRowByIndex(dg, dg.SelectedIndex);
		}

		private static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
		{
			DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

			// https://stackoverflow.com/a/27792628/5740031
			if (row == null)
			{
				dataGrid.UpdateLayout();
				dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
				row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			}

			if (row != null)
			{
				DataGridCell cell = GetCell(dataGrid, row, 0);
				cell?.Focus();
			}
		}

		private static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
			if (presenter == null)
			{
				// If the row has been virtualized away, call its ApplyTemplate() method
				// to build its visual tree in order for the DataGridCellsPresenter
				// and the DataGridCells to be created
				rowContainer.ApplyTemplate();
				presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
			}

			if (presenter != null)
			{
				DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
				if (cell == null)
				{
					// Bring the column into view in case it has been virtualized away
					dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
					cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
				}

				return cell;
			}

			return null;
		}

		private static T FindVisualChild<T>(DependencyObject obj)
			where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				var visualChild = child as T;
				if (visualChild != null)
				{
					return visualChild;
				}

				T childOfChild = FindVisualChild<T>(child);
				if (childOfChild != null)
				{
					return childOfChild;
				}
			}

			return null;
		}

		private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var frameworkElement = e.Source as FrameworkElement;
			var viewModel = DataContext as LibraryExplorerViewModel;
			if (frameworkElement == null || viewModel == null)
			{
				return;
			}

			if (viewModel.SelectedItem is DiscExplorerItem)
			{
				ContextMenu contextMenu = new ContextMenu();
				contextMenu.Items.Add(
					new MenuItem
					{
						Header = "Delete Disc",
						Command = viewModel.DeleteDiscCommand,
					});
				contextMenu.Items.Add(
					new MenuItem
					{
						Header = "Properties",
						Command = viewModel.EditDiscPropertiesCommand,
					});

				frameworkElement.ContextMenu = contextMenu;
			}
			else
			{
				frameworkElement.ContextMenu = null;
			}
		}

		private void ContentDataGrid_OnKeyDown(object sender, KeyEventArgs e)
		{
			var enteredText = GetTextFromKey(e.Key);
			if (String.IsNullOrEmpty(enteredText))
			{
				return;
			}

			DataGrid dataGrid = ContentDataGrid;
			var selected = dataGrid.Items.Cast<LibraryExplorerItem>()
				.FirstOrDefault(it => it.Name.StartsWith(enteredText, StringComparison.CurrentCultureIgnoreCase));
			if (selected != null)
			{
				dataGrid.SelectedItem = selected;
			}
		}

		// https://stackoverflow.com/a/5826175/5740031
		public static string GetTextFromKey(Key key)
		{
			int virtualKey = KeyInterop.VirtualKeyFromKey(key);
			byte[] keyboardState = new byte[256];
			NativeMethods.GetKeyboardState(keyboardState);

			uint scanCode = NativeMethods.MapVirtualKey((uint)virtualKey, NativeMethods.MapType.MAPVK_VK_TO_VSC);
			StringBuilder stringBuilder = new StringBuilder(2);

			int result = NativeMethods.ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
			return result >= 1 ? stringBuilder.ToString() : String.Empty;
		}
	}
}
