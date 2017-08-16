using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	internal class PlayDiscEventArgs : EventArgs
	{
		public Disc Disc { get; }

		public PlayDiscEventArgs(Disc disc)
		{
			Disc = disc;
		}
	}
}
