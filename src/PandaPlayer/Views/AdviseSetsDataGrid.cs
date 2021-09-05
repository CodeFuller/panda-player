using System;
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
	}
}
