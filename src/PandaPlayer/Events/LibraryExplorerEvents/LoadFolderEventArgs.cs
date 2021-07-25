using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.LibraryExplorerEvents
{
	public class LoadFolderEventArgs : EventArgs
	{
		public ItemId FolderId { get; set; }

		public LoadFolderEventArgs(ItemId folderId)
		{
			FolderId = folderId ?? throw new ArgumentNullException(nameof(folderId));
		}
	}
}
