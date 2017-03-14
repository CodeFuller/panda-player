using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
{
	/// <summary>
	/// Interaction logic for AlbumTreeView.xaml
	/// </summary>
	public partial class AlbumTreeView : TreeView, IScrollable
	{
		private readonly Lazy<ScrollViewer> scrollViewer;

		public AlbumTreeView()
		{
			InitializeComponent();

			scrollViewer = new Lazy<ScrollViewer>(() => FindScroller(this));
		}

		public void ScrollTo(double offset)
		{
			scrollViewer.Value?.ScrollToVerticalOffset(offset);
		}

		private ScrollViewer FindScroller(DependencyObject item)
		{
			var scroller = item as ScrollViewer;

			for (var i = 0; scroller == null && i < VisualTreeHelper.GetChildrenCount(item); ++i)
			{
				var child = VisualTreeHelper.GetChild(item, i);
				if (child != null)
				{
					scroller = FindScroller(child);
				}
			}

			return scroller;
		}
	}
}
