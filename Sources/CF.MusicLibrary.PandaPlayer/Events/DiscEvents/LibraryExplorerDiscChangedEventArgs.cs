using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
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
