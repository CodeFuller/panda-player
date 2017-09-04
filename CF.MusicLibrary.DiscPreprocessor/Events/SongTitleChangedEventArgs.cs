namespace CF.MusicLibrary.DiscPreprocessor.Events
{
	public class SongTitleChangedEventArgs : SongTitleChangeEventArgs
	{
		public SongTitleChangedEventArgs(string oldTitle, string newTitle) :
			base(oldTitle, newTitle)
		{
		}
	}
}
