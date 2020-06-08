using System;

namespace MusicLibrary.DiscPreprocessor.Events
{
	public abstract class SongTitleChangeEventArgs : EventArgs
	{
		public string OldTitle { get; set; }

		public string NewTitle { get; set; }

		protected SongTitleChangeEventArgs(string oldTitle, string newTitle)
		{
			OldTitle = oldTitle;
			NewTitle = newTitle;
		}
	}
}
