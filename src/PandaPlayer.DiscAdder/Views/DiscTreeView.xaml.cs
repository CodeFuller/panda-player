using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PandaPlayer.DiscAdder.Views
{
	public partial class DiscTreeView : TreeView, IScrollable
	{
		private readonly Lazy<ScrollViewer> scrollViewer;

		public DiscTreeView()
		{
			InitializeComponent();

			scrollViewer = new Lazy<ScrollViewer>(() => FindScrollViewer(this));
		}

		public void ScrollTo(double offset)
		{
			scrollViewer.Value?.ScrollToVerticalOffset(offset);
		}

		private static ScrollViewer FindScrollViewer(DependencyObject item)
		{
			var scrollViewer = item as ScrollViewer;

			for (var i = 0; scrollViewer == null && i < VisualTreeHelper.GetChildrenCount(item); ++i)
			{
				var child = VisualTreeHelper.GetChild(item, i);
				scrollViewer = FindScrollViewer(child);
			}

			return scrollViewer;
		}
	}
}
