using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.DiscEvents
{
	public class NavigateLibraryExplorerToDiscEventArgs : BaseDiscEventArgs
	{
		public NavigateLibraryExplorerToDiscEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}
