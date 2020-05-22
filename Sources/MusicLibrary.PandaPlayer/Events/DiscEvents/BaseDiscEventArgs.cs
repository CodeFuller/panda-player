using System;
using MusicLibrary.Logic.Models;

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
