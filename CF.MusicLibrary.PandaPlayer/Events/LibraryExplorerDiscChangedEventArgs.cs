using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class LibraryExplorerDiscChangedEventArgs : EventArgs
	{
		public Disc Disc { get; }

		public LibraryExplorerDiscChangedEventArgs(Disc disc)
		{
			Disc = disc;
		}
	}
}
