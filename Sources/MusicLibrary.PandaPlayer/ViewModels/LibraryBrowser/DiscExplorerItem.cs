using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : BasicExplorerItem
	{
		public DiscModel Disc { get; }

		public ItemId DiscId => Disc.Id;

		public override string Title => Disc.TreeTitle;

		public DiscExplorerItem(DiscModel disc)
		{
			this.Disc = disc ?? throw new ArgumentNullException(nameof(disc));
		}
	}
}
