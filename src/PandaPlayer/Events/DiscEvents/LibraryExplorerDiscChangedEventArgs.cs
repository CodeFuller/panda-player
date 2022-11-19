using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public class LibraryExplorerDiscChangedEventArgs : BaseDiscEventArgs
	{
		public bool DeletedContentIsShown { get; }

		public LibraryExplorerDiscChangedEventArgs(DiscModel disc, bool deletedContentIsShown)
			: base(disc)
		{
			DeletedContentIsShown = deletedContentIsShown;
		}
	}
}
