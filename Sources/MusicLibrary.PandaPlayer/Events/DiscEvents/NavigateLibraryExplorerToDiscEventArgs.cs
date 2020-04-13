using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class NavigateLibraryExplorerToDiscEventArgs : BaseDiscEventArgs
	{
		public NavigateLibraryExplorerToDiscEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
