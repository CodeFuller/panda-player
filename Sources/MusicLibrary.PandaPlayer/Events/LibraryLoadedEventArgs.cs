using System;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events
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
