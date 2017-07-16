using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
{
	/// <summary>
	/// Interaction logic for EditSongsDetailsWindow.xaml
	/// </summary>
	public partial class EditSongsDetailsWindow : Window
	{
		public EditSongsDetailsWindow(IEditSongsDetailsViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}

		private void ButtonAddToLibrary_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
