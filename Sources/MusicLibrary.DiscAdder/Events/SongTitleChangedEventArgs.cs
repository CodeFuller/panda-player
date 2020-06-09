namespace MusicLibrary.DiscAdder.Events
{
	internal class SongTitleChangedEventArgs : SongTitleChangeEventArgs
	{
		public SongTitleChangedEventArgs(string oldTitle, string newTitle)
			: base(oldTitle, newTitle)
		{
		}
	}
}
