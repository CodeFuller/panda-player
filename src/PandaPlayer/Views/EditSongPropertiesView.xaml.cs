using System.Threading;
using System.Windows;
using System.Windows.Input;
using CodeFuller.Library.Wpf.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	public partial class EditSongPropertiesView : Window
	{
		private IEditSongPropertiesViewModel ViewModel => DataContext.GetViewModel<IEditSongPropertiesViewModel>();

		public EditSongPropertiesView()
		{
			InitializeComponent();
		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.IsValid();
		}

		private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await ViewModel.Save(CancellationToken.None);
			DialogResult = true;
			e.Handled = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
