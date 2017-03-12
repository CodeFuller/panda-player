using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
{
	interface IScrollable
	{
		void ScrollTo(double offset);
	}

	class ScrollableTextBox : IScrollable
	{
		private readonly TextBox textBox;

		public ScrollableTextBox(TextBox textBox)
		{
			this.textBox = textBox;
		}

		public void ScrollTo(double offset)
		{
			textBox.ScrollToVerticalOffset(offset);
		}
	}

	class ScrollViewerScrollable : IScrollable
	{
		private readonly ScrollViewer scrollViewer;

		public ScrollViewerScrollable(ScrollViewer scrollViewer)
		{
			this.scrollViewer = scrollViewer;
		}

		public void ScrollTo(double offset)
		{
			scrollViewer.ScrollToVerticalOffset(offset);
		}
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ScrollViewer treeViewScroller;

		public MainWindowModel Model => (MainWindowModel)Resources["ViewModel"];

		public MainWindow()
		{
			InitializeComponent();
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

		private void ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (treeViewScroller == null)
			{
				treeViewScroller = FindScroller(TreeViewCurrentAlbumsContent);
			}

			List<IScrollable> scrolledControls = new List<IScrollable>
			{
				new ScrollableTextBox(TextBoxRawEthalonAlbums),
				new ScrollableTextBox(TextBoxParsedEthalonAlbums),
			};
			if (treeViewScroller != null)
			{
				scrolledControls.Add(new ScrollViewerScrollable(treeViewScroller));
			}

			scrolledControls.RemoveAll(x => x == sender);
			foreach (var control in scrolledControls)
			{
				control.ScrollTo(e.VerticalOffset);
			}
		}
	}
}
