using System.Windows;
using MusicLibrary.DiscAdder.ViewModels;

namespace MusicLibrary.DiscAdder.Views
{
	internal partial class DiscAdderView : Window
	{
		public DiscAdderView(DiscAdderViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}
	}
}
