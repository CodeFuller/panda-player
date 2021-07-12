using System.Windows;

namespace PandaPlayer.Views
{
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
