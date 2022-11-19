using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public class ActiveDiscChangedEventArgs : BaseDiscEventArgs
	{
		public ActiveDiscChangedEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}
