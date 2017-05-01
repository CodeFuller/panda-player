using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
{
	/// <summary>
	/// Interaction logic for AddToLibraryWindow.xaml
	/// </summary>
	public partial class AddToLibraryWindow : Window
	{
		public AddToLibraryWindow(AddToLibraryViewModel model)
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
