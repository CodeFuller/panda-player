using System.Collections.Generic;
using System.Windows.Controls;

namespace PandaPlayer.DiscAdder.Views
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
				new ScrollableTextBox(RawReferenceContentTextBox),
				ReferenceContentTreeView,
				ActualContentTreeView,
			};

			scrolledControls.RemoveAll(x => x == sender);
			foreach (var control in scrolledControls)
			{
				control.ScrollTo(e.VerticalOffset);
			}
		}
	}
}
