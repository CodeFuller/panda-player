using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public class DiscImageChangedEventArgs : BaseDiscEventArgs
	{
		public DiscImageChangedEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}
