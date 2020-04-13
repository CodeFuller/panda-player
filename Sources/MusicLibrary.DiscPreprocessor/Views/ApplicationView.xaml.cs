using System.Windows;
using MusicLibrary.DiscPreprocessor.ViewModels;

namespace MusicLibrary.DiscPreprocessor.Views
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
