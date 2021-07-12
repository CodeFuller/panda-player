namespace PandaPlayer.Views
{
	// It is not possible to have XAML for UserControl which inherits from another UserControl.
	// That's why we define this control only via code-behind.
	// https://stackoverflow.com/questions/269106/
	public class PlaylistView : SongListView
	{
		public PlaylistView()
		{
			InitializeComponent();

			this.Loaded += (_, _) =>
			{
				SongsDataGrid.ContextMenu = new PlaylistContextMenu();
			};
		}
	}
}
