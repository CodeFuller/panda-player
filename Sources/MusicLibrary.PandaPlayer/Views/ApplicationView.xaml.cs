using System.Windows;
using System.Windows.Input;
using MusicLibrary.PandaPlayer.ViewModels;

namespace MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for ApplicationView.xaml
	/// </summary>
	public partial class ApplicationView : Window
	{
		public ApplicationView(ApplicationViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}

		private void DiscImageView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DiscImageView.Image_MouseLeftButtonDown(sender, e);
		}
	}
}
