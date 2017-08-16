using System.Windows;
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
	}
}
