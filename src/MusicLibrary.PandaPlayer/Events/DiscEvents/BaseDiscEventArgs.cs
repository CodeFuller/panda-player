using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
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
