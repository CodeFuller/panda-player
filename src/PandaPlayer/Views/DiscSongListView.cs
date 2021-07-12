namespace PandaPlayer.Views
{
	// It is not possible to have XAML for UserControl which inherits from another UserControl.
	// That's why we define this control only via code-behind.
	// https://stackoverflow.com/questions/269106/
	public class DiscSongListView : SongListView
	{
		public DiscSongListView()
		{
			InitializeComponent();

			this.Loaded += (_, _) =>
			{
				SongsDataGrid.ContextMenu = new DiscSongListContextMenu();
			};
		}
	}
}
