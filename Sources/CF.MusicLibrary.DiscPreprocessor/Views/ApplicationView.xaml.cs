using System.Windows;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;

namespace CF.MusicLibrary.DiscPreprocessor.Views
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
