using System.Windows;
using MusicLibrary.DiscAdder.ViewModels;

namespace MusicLibrary.DiscAdder.Views
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
