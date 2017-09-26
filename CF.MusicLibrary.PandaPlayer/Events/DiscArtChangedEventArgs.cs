using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class DiscArtChangedEventArgs : EventArgs
	{
		public Disc Disc { get; }

		public DiscArtChangedEventArgs(Disc disc)
		{
			Disc = disc;
		}
	}
}
