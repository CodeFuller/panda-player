using System.Windows;
using System.Windows.Controls;
using PandaPlayer.ViewModels;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	public partial class LoggerView : UserControl
	{
		public LoggerView()
		{
			InitializeComponent();
		}

		private void View_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is LoggerViewModel viewModel)
			{
				viewModel.Messages.CollectionChanged += (s, ev) => MessagesDataGrid.ScrollToDataGridBottom();
			}
		}
	}
}
