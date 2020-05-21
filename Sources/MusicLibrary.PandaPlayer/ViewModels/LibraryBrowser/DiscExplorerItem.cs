using System;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : BasicExplorerItem
	{
		private readonly FolderDiscModel disc;

		public ItemId DiscId => disc.Id;

		public override string Title => disc.TreeTitle;

		// TBD: Remove after redesign
		public DiscModel Disc => disc.Disc;

		public DiscExplorerItem(FolderDiscModel disc)
		{
			this.disc = disc ?? throw new ArgumentNullException(nameof(disc));
		}
	}
}
