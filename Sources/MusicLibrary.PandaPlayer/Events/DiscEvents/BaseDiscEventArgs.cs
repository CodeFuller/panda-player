using System;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public abstract class BaseDiscEventArgs : EventArgs
	{
		public ItemId DiscId { get; }

		protected BaseDiscEventArgs(ItemId discId)
		{
			// discId can be null.
			DiscId = discId;
		}
	}
}
