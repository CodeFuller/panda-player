using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public abstract class BaseDiscEventArgs : EventArgs
	{
		public DiscModel Disc { get; }

		protected BaseDiscEventArgs(DiscModel disc)
		{
			// It can be null.
			Disc = disc;
		}
	}
}
