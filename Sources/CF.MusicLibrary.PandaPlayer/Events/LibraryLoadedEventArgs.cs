using System;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class LibraryLoadedEventArgs : EventArgs
	{
		public DiscLibrary DiscLibrary { get; }

		public LibraryLoadedEventArgs(DiscLibrary discLibrary)
		{
			DiscLibrary = discLibrary;
		}
	}
}
