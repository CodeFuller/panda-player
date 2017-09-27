using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class ActiveDiscChangedEventArgs : EventArgs
	{
		public Disc Disc { get; }

		public ActiveDiscChangedEventArgs(Disc disc)
		{
			Disc = disc;
		}
	}
}
