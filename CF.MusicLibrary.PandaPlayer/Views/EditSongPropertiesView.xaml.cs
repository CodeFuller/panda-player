using System.Windows;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for EditSongPropertiesView.xaml
	/// </summary>
	public partial class EditSongPropertiesView : Window
	{
		public EditSongPropertiesView()
		{
			InitializeComponent();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
