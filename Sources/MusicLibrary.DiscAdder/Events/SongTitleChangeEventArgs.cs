using System;

namespace MusicLibrary.DiscAdder.Events
{
	internal abstract class SongTitleChangeEventArgs : EventArgs
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
