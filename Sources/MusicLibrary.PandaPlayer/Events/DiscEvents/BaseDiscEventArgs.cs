using System;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
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
