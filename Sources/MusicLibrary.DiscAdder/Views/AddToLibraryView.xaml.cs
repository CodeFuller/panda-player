using System.Windows.Controls;

namespace MusicLibrary.DiscPreprocessor.Views
{
	/// <summary>
	/// Interaction logic for AddToLibraryView.xaml
	/// </summary>
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
