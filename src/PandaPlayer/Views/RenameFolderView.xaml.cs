using System.Threading;
using System.Windows;
using System.Windows.Input;
using CodeFuller.Library.Wpf.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	public partial class RenameFolderView : Window
	{
		private IRenameFolderViewModel ViewModel => DataContext.GetViewModel<IRenameFolderViewModel>();

		public RenameFolderView()
		{
			InitializeComponent();
		}

		private void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.IsValid();
		}

		private async void Rename_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await ViewModel.Rename(CancellationToken.None);

			DialogResult = true;
			e.Handled = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
