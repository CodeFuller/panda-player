using System.Windows.Controls;
using System.Windows.Media;

namespace MusicLibrary.PandaPlayer.Views.Extensions
{
	internal static class DataGridExtensions
	{
		public static void ScrollToDataGridBottom(this DataGrid dataGrid)
		{
			var border = VisualTreeHelper.GetChild(dataGrid, 0) as Decorator;
			var scroll = border?.Child as ScrollViewer;
			scroll?.ScrollToEnd();
		}
	}
}
