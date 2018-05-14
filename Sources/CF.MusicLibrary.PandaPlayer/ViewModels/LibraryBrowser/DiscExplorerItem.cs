using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : FolderExplorerItem
	{
		public Disc Disc { get; }

		public DiscExplorerItem(Disc disc)
			: base(disc.Uri)
		{
			Disc = disc;
		}
	}
}
