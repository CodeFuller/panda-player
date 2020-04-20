using System;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Dto.Folders;

namespace MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FolderExplorerItem : BasicExplorerItem
	{
		private readonly SubfolderData folderData;

		public ItemId FolderId => folderData.Id;

		public override string Title => folderData.Name;

		public FolderExplorerItem(SubfolderData folderData)
		{
			this.folderData = folderData ?? throw new ArgumentNullException(nameof(folderData));
		}
	}
}
