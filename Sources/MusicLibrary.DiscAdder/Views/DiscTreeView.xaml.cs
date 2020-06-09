using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicLibrary.DiscAdder.Views
{
	/// <summary>
	/// Interaction logic for DiscTreeView.xaml
	/// </summary>
	public partial class DiscTreeView : TreeView, IScrollable
	{
		private readonly Lazy<ScrollViewer> scrollViewer;

		public DiscTreeView()
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
