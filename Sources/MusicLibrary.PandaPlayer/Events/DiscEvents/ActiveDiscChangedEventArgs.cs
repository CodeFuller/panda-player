using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class ActiveDiscChangedEventArgs : BaseDiscEventArgs
	{
		public ActiveDiscChangedEventArgs(ItemId discId)
			: base(discId)
		{
		}
	}
}
