namespace CF.MusicLibrary.AlbumPreprocessor.Events
{
	public class SongTitleChangingEventArgs : SongTitleChangeEventArgs
	{
		public SongTitleChangingEventArgs(string oldTitle, string newTitle) :
			base(oldTitle, newTitle)
		{
		}
	}
}
