using System.Windows;
using PandaPlayer.ViewModels;

namespace PandaPlayer.Views
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
