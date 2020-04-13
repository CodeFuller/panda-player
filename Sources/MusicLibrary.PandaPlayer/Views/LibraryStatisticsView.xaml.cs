using System.Windows;

namespace MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for LibraryStatisticsView.xaml
	/// </summary>
	public partial class LibraryStatisticsView : Window
	{
		public LibraryStatisticsView()
		{
			InitializeComponent();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
