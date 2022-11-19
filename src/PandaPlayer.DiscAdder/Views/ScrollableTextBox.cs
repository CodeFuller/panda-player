using System.Windows.Controls;

namespace PandaPlayer.DiscAdder.Views
{
	internal class ScrollableTextBox : IScrollable
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
}
