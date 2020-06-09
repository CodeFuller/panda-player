using System.Windows;
using MusicLibrary.DiscAdder.ViewModels;

namespace MusicLibrary.DiscAdder.Views
{
	internal partial class ApplicationView : Window
	{
		public ApplicationView(ApplicationViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}
	}
}
