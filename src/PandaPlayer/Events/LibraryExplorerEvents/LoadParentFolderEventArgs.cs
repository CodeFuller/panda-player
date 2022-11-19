using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.LibraryExplorerEvents
{
	public class LoadParentFolderEventArgs : EventArgs
	{
		public FolderModel ParentFolder { get; set; }

		public ItemId ChildFolderId { get; set; }

		public LoadParentFolderEventArgs(FolderModel parentFolder, ItemId childFolderId)
		{
			ParentFolder = parentFolder ?? throw new ArgumentNullException(nameof(parentFolder));
			ChildFolderId = childFolderId ?? throw new ArgumentNullException(nameof(childFolderId));
		}
	}
}
