using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
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
