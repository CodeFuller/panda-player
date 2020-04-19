using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : LibraryExplorerItem
	{
		public Disc Disc { get; }

		public DiscExplorerItem(Disc disc)
			: base(disc.Uri)
		{
			Disc = disc;
		}
	}
}
