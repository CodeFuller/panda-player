using System.Windows;
using MusicLibrary.PandaPlayer.ViewModels;

namespace MusicLibrary.PandaPlayer.Views
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
