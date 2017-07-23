using System.Windows.Controls;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
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
