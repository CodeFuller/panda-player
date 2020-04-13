using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MusicLibrary.PandaPlayer.Views
{
	// https://stackoverflow.com/a/22908694/5740031
	public class MultiSelectionDataGrid : DataGrid
	{
		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(MultiSelectionDataGrid), new PropertyMetadata(null));

#pragma warning disable CA2227 // Collection properties should be read only - Collection is used in two-way binding
		public IList SelectedItemsList
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => (IList)GetValue(SelectedItemsListProperty);
			set => SetValue(SelectedItemsListProperty, value);
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
