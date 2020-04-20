using System;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Dto.Folders;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class DiscExplorerItem : BasicExplorerItem
	{
		private readonly FolderDiscData discData;

		public ItemId DiscId => discData.Id;

		public override string Title => discData.TreeTitle;

		// TBD: Remove after redesign
		public Disc Disc => discData.Disc;

		public DiscExplorerItem(FolderDiscData discData)
		{
			this.discData = discData ?? throw new ArgumentNullException(nameof(discData));
		}
	}
}
