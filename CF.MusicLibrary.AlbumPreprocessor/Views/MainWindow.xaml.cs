using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindowModel Model { get; }

		public MainWindow(MainWindowModel model)
		{
			InitializeComponent();
			DataContext = Model = model;
		}

		private void ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			List<IScrollable> scrolledControls = new List<IScrollable>
			{
				new ScrollableTextBox(TextBoxRawEthalonAlbums),
				TreeViewEthalonAlbumsContent,
				TreeViewCurrentAlbumsContent,
			};

			scrolledControls.RemoveAll(x => x == sender);
			foreach (var control in scrolledControls)
			{
				control.ScrollTo(e.VerticalOffset);
			}
		}
	}
}
