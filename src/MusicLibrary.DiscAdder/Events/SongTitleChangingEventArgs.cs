namespace MusicLibrary.DiscAdder.Events
{
	internal class SongTitleChangingEventArgs : SongTitleChangeEventArgs
	{
		public SongTitleChangingEventArgs(string oldTitle, string newTitle)
			: base(oldTitle, newTitle)
		{
		}
	}
}
