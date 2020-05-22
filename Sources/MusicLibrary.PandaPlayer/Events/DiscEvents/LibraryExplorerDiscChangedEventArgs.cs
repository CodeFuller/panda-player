using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class LibraryExplorerDiscChangedEventArgs : BaseDiscEventArgs
	{
		public LibraryExplorerDiscChangedEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}
