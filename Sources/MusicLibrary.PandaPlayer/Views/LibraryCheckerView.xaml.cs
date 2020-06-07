using System.Windows;
using System.Windows.Input;
using MusicLibrary.PandaPlayer.ViewModels;
using MusicLibrary.PandaPlayer.Views.Extensions;

namespace MusicLibrary.PandaPlayer.Views
{
	public partial class LibraryCheckerView : Window
	{
		public LibraryCheckerView()
		{
			InitializeComponent();
		}

		private void View_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is LibraryCheckerViewModel viewModel)
			{
				viewModel.Inconsistencies.CollectionChanged += (s, ev) => MessagesDataGrid.ScrollToDataGridBottom();
			}
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
