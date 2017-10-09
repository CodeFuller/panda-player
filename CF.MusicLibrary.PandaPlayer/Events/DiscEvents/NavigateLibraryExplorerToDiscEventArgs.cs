using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class NavigateLibraryExplorerToDiscEventArgs : BaseDiscEventArgs
	{
		public NavigateLibraryExplorerToDiscEventArgs(Disc disc)
			: base(disc)
		{
		}
	}
}
