using System.Windows;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for RateDiscView.xaml
	/// </summary>
	public partial class RateDiscView : Window
	{
		public RateDiscView()
		{
			InitializeComponent();
		}

		private void RateButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
