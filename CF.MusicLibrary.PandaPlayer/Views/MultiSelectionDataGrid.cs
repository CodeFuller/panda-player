using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <remarks>
	/// https://stackoverflow.com/a/22908694/5740031
	/// </remarks>>
	public class MultiSelectionDataGrid : DataGrid
	{
		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(MultiSelectionDataGrid), new PropertyMetadata(null));

		public IList SelectedItemsList
		{
			get { return (IList)GetValue(SelectedItemsListProperty); }
			set { SetValue(SelectedItemsListProperty, value); }
		}

		public MultiSelectionDataGrid()
		{
			SelectionChanged += CustomDataGrid_SelectionChanged;
		}

		private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}
	}
}
