using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using PandaPlayer.ViewModels.AdviseSetsEditor;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.Views
{
	public class AdviseSetsDataGrid : DataGrid
	{
		private IAdviseSetsEditorViewModel ViewModel => DataContext as IAdviseSetsEditorViewModel;

		private bool IsEditMode => ((IEditableCollectionView)Items).IsEditingItem;

		public AdviseSetsDataGrid()
		{
			WeakReferenceMessenger.Default.Register<AdviseSetCreatedEventArgs>(this, (_, e) => OnAdviseSetCreated(e));
		}

		private void OnAdviseSetCreated(AdviseSetCreatedEventArgs e)
		{
			var rowIndex = ViewModel.AdviseSets.IndexOf(e.AdviseSet);
			if (rowIndex == -1)
			{
				return;
			}

			// https://stackoverflow.com/questions/1629311/
			CurrentCell = new DataGridCellInfo(Items[rowIndex], Columns[0]);

			// https://stackoverflow.com/questions/46201079/
			Dispatcher.BeginInvoke(new Action(() => BeginEdit()), DispatcherPriority.Background);
		}

		protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
		{
			var adviseSet = ViewModel.SelectedAdviseSet;

			base.OnExecutedCommitEdit(e);

			ViewModel.RenameAdviseSet(adviseSet, CancellationToken.None)
				.GetAwaiter().GetResult();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			// DataGrid behavior is similar to Excel sheets:
			// When Enter key is pressed, the focus is switched to the next row and same column.
			// If Shift+Enter is pressed, the focus is switched to the previous row and same column.
			// If Ctrl+Enter is pressed, the focus is left at the current cell.
			// This code is located here: https://github.com/dotnet/wpf/blob/76864c4b41a0e9f6070f4f98af5573a54234e201/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/DataGrid.cs#L5981
			// We want to adjust this behavior, so that when Enter is pressed, the focus is left at the current cell.
			// We want to keep current advise set active, so that user can continue editing its discs.
			// To achieve this, we call CommitEdit for the row ( )which effectively happens for DataGrid when Enter is pressed) and mark event as handled.
			if (IsEditMode && e.Key == Key.Enter)
			{
				CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
				e.Handled = true;
			}

			if (!IsEditMode && e.Key == Key.Escape)
			{
				if (ItemContainerGenerator.ContainerFromItem(SelectedItem) is DataGridRow row)
				{
					row.IsSelected = false;
				}
			}
		}

		// Clear selection if user clicks on DataGrid whitespace.
		// https://stackoverflow.com/a/18066728/5740031
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (IsEditMode)
			{
				return;
			}

			var row = ItemContainerGenerator.ContainerFromItem(SelectedItem) as DataGridRow;
			if (!row?.IsMouseOver ?? false)
			{
				row.IsSelected = false;
			}
		}
	}
}
