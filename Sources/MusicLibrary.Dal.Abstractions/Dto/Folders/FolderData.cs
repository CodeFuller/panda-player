using System.Collections.Generic;

namespace MusicLibrary.Dal.Abstractions.Dto.Folders
{
	public class FolderData
	{
		public ItemId Id { get; set; }

		public ItemId ParentFolderId { get; set; }

		public IReadOnlyCollection<SubfolderData> Subfolders { get; set; }

		public IReadOnlyCollection<FolderDiscData> Discs { get; set; }
	}
}
