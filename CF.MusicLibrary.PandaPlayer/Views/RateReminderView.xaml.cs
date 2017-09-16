using System.Windows;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for RateReminderView.xaml
	/// </summary>
	public partial class RateReminderView : Window
	{
		public RateReminderView()
		{
			InitializeComponent();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
