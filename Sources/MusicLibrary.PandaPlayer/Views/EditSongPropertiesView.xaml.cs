using System.Windows;
using System.Windows.Input;
using CF.Library.Wpf.Extensions;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for EditSongPropertiesView.xaml
	/// </summary>
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
			await ViewModel.Save();
			DialogResult = true;
			e.Handled = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
