using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public class LibraryExplorerDiscChangedEventArgs : BaseDiscEventArgs
	{
		public LibraryExplorerDiscChangedEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}
