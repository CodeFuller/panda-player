using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : FolderExplorerItem
	{
		public Disc Disc { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "No way to check argument passed to base constructor")]
		public DiscExplorerItem(Disc disc)
			: base(disc.Uri)
		{
			Disc = disc;
		}
	}
}
