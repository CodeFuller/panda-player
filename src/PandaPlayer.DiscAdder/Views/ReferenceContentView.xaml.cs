using System;
using System.Windows.Controls;
using PandaPlayer.DiscAdder.Views.Extensions;

namespace PandaPlayer.DiscAdder.Views
{
	public partial class ReferenceContentView : TreeView, IScrollable
	{
		private readonly Lazy<ScrollViewer> scrollViewer;

		public ReferenceContentView()
		{
			InitializeComponent();

			scrollViewer = new Lazy<ScrollViewer>(this.FindScrollViewer);
		}

		public void ScrollTo(double offset)
		{
			scrollViewer.Value?.ScrollToVerticalOffset(offset);
		}
	}
}
