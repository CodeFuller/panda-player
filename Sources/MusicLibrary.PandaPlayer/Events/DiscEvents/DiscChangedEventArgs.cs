using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	internal class DiscChangedEventArgs : BaseDiscEventArgs
	{
		public string PropertyName { get; }

		public DiscChangedEventArgs(DiscModel disc, string propertyName)
			: base(disc)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
		}
	}
}
