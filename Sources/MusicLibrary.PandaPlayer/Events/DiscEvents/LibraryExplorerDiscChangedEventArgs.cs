using System;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class LibraryExplorerDiscChangedEventArgs : EventArgs
	{
		public DiscModel Disc { get; }

		public LibraryExplorerDiscChangedEventArgs(DiscModel disc)
		{
			// Disc can be null.
			this.Disc = disc;
		}
	}
}
