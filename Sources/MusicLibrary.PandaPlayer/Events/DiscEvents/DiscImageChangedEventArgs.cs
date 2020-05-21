using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class DiscImageChangedEventArgs : BaseDiscEventArgs
	{
		public DiscImageChangedEventArgs(ItemId discId)
			: base(discId)
		{
		}
	}
}
