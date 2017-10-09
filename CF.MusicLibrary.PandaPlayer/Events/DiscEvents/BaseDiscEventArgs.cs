using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public abstract class BaseDiscEventArgs : EventArgs
	{
		public Disc Disc { get; }

		protected BaseDiscEventArgs(Disc disc)
		{
			Disc = disc;
		}
	}
}
