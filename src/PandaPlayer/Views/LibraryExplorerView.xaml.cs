﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.Views
{
	public partial class LibraryExplorerView : UserControl
	{
		public LibraryExplorerView()
		{
			InitializeComponent();
		}

		// https://stackoverflow.com/a/29081353/5740031
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is not DataGrid dg || dg.SelectedIndex < 0)
			{
				return;
			}

			SelectRowByIndex(dg, dg.SelectedIndex);
		}

		private static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
		{
			// https://stackoverflow.com/a/27792628/5740031
			if (dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) is not DataGridRow row)
			{
				dataGrid.UpdateLayout();
				dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
				row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			}

			if (row != null)
			{
				var cell = GetCell(dataGrid, row, 0);
				cell?.Focus();
			}
		}

		private static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			var presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
			if (presenter == null)
			{
				// If the row has been virtualized away, call its ApplyTemplate() method
				// to build its visual tree in order for the DataGridCellsPresenter
				// and the DataGridCells to be created
				rowContainer.ApplyTemplate();
				presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					return null;
				}
			}

			if (presenter.ItemContainerGenerator.ContainerFromIndex(column) is not DataGridCell cell)
			{
				// Bring the column into view in case it has been virtualized away
				dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
				cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
			}

			return cell;
		}

		private static T FindVisualChild<T>(DependencyObject obj)
			where T : DependencyObject
		{
			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
			{
				var child = VisualTreeHelper.GetChild(obj, i);
				if (child is T visualChild)
				{
					return visualChild;
				}

				var childOfChild = FindVisualChild<T>(child);
				if (childOfChild != null)
				{
					return childOfChild;
				}
			}

			return null;
		}

		private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (e.Source is not FrameworkElement frameworkElement || DataContext is not ILibraryExplorerViewModel viewModel)
			{
				return;
			}

			var contextMenu = new ContextMenu();
			foreach (var menuItem in viewModel.ContextMenuItemsForSelectedItem.Select(x => x.MenuItemControl))
			{
				contextMenu.Items.Add(menuItem);
			}

			frameworkElement.ContextMenu = contextMenu;
		}

		private void ContentDataGrid_OnKeyDown(object sender, KeyEventArgs e)
		{
			var enteredText = GetTextFromKey(e.Key);
			if (String.IsNullOrEmpty(enteredText))
			{
				return;
			}

			var dataGrid = ContentDataGrid;
			var selected = dataGrid.Items.Cast<BasicExplorerItem>()
				.FirstOrDefault(it => it.Title.StartsWith(enteredText, StringComparison.CurrentCultureIgnoreCase));
			if (selected != null)
			{
				dataGrid.SelectedItem = selected;
			}
		}

		// https://stackoverflow.com/a/5826175/5740031
		public static string GetTextFromKey(Key key)
		{
			var virtualKey = KeyInterop.VirtualKeyFromKey(key);
			var keyboardState = new byte[256];
			NativeMethods.GetKeyboardState(keyboardState);

			var scanCode = NativeMethods.MapVirtualKey((uint)virtualKey, NativeMethods.MapType.MAPVK_VK_TO_VSC);
			var stringBuilder = new StringBuilder(2);

			var result = NativeMethods.ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
			return result >= 1 ? stringBuilder.ToString() : String.Empty;
		}

		private void ContentDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (DataContext is not ILibraryExplorerViewModel viewModel)
			{
				return;
			}

			viewModel.ItemListViewModel.ChangeFolderCommand.Execute(null);
		}
	}
}
