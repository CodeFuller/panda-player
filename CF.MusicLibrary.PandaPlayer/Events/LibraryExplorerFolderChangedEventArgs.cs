using System;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class LibraryExplorerFolderChangedEventArgs : EventArgs
	{
		public Uri FolderUri { get; }

		public LibraryExplorerFolderChangedEventArgs(Uri folderUri)
		{
			FolderUri = folderUri;
		}
	}
}
