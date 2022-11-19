using System.Windows.Controls;

namespace PandaPlayer.DiscAdder.Views
{
	public partial class AddToLibraryView : UserControl
	{
		public AddToLibraryView()
		{
			InitializeComponent();
		}

		private void TextBoxProgressMessages_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			TextBoxProgressMessages.ScrollToEnd();
		}
	}
}
