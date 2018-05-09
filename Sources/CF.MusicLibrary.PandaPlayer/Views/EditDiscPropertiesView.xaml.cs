using System.Windows;
using System.Windows.Input;
using CF.Library.Wpf.Extensions;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for EditDiscPropertiesView.xaml
	/// </summary>
	public partial class EditDiscPropertiesView : Window
	{
		private IEditDiscPropertiesViewModel ViewModel => DataContext.GetViewModel<IEditDiscPropertiesViewModel>();

		public EditDiscPropertiesView()
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
