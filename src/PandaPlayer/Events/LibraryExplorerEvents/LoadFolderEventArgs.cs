using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.LibraryExplorerEvents
{
	public class LoadFolderEventArgs : EventArgs
	{
		public FolderModel Folder { get; set; }

		public LoadFolderEventArgs(FolderModel folder)
		{
			Folder = folder ?? throw new ArgumentNullException(nameof(folder));
		}
	}
}
