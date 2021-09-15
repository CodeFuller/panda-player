using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
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
			Messenger.Default.Register<AdviseSetCreatedEventArgs>(this, OnAdviseSetCreated);
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

		// DataGrid behavior is similar to Excel sheets:
		// When Enter key is pressed, the focus is switched to the next row and same column.
		// If Shift+Enter is pressed, the focus is switched to the previous row and same column.
		// If Ctrl+Enter is pressed, the focus is left at the current cell.
		// This code is located here: https://github.com/dotnet/wpf/blob/76864c4b41a0e9f6070f4f98af5573a54234e201/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/DataGrid.cs#L5981
		// We want to adjust this behavior, so that when Enter is pressed, the focus is left at the current cell.
		// We want to keep current advise set active, so that user can continue editing its discs.
		// To achieve this, we call CommitEdit for the row ( )which effectively happens for DataGrid when Enter is pressed) and mark event as handled.
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (IsEditMode && e.Key == Key.Enter)
			{
				CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
				e.Handled = true;
			}
		}
	}
}
