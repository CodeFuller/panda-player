using System.Collections.Generic;
using System.Windows.Controls;

namespace CF.MusicLibrary.DiscPreprocessor.Views
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
	/// Interaction logic for EditSourceContentView.xaml
	/// </summary>
	public partial class EditSourceContentView : UserControl
	{
		public EditSourceContentView()
		{
			InitializeComponent();
		}

		private void ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			List<IScrollable> scrolledControls = new List<IScrollable>
			{
				new ScrollableTextBox(TextBoxRawEthalonDiscs),
				TreeViewEthalonDiscsContent,
				TreeViewCurrentDiscsContent,
			};

			scrolledControls.RemoveAll(x => x == sender);
			foreach (var control in scrolledControls)
			{
				control.ScrollTo(e.VerticalOffset);
			}
		}
	}
}
