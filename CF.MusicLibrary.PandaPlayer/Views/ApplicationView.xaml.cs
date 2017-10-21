using System.Windows;
using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer.Views
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Class is called from XAML")]
		private void DiscImageView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DiscImageView.Image_MouseLeftButtonDown(sender, e);
		}
	}
}
