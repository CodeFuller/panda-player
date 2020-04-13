using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class LibraryExplorerDiscChangedEventArgs
		: BaseDiscEventArgs
	{
		public LibraryExplorerDiscChangedEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
