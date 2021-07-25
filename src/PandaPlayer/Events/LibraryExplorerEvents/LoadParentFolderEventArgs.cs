using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.LibraryExplorerEvents
{
	public class LoadParentFolderEventArgs : EventArgs
	{
		public ItemId ParentFolderId { get; set; }

		public ItemId ChildFolderId { get; set; }

		public LoadParentFolderEventArgs(ItemId parentFolderId, ItemId childFolderId)
		{
			ParentFolderId = parentFolderId ?? throw new ArgumentNullException(nameof(parentFolderId));
			ChildFolderId = childFolderId ?? throw new ArgumentNullException(nameof(childFolderId));
		}
	}
}
