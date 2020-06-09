using System.Collections.Generic;
using System.Windows.Controls;

namespace MusicLibrary.DiscAdder.Views
{
	public partial class EditSourceContentView : UserControl
	{
		public EditSourceContentView()
		{
			InitializeComponent();
		}

		private void ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			var scrolledControls = new List<IScrollable>
			{
				new ScrollableTextBox(TextBoxRawReferenceDiscs),
				TreeViewReferenceDiscsContent,
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
