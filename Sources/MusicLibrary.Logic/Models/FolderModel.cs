using System.Collections.Generic;

namespace MusicLibrary.Logic.Models
{
	public class FolderModel
	{
		public ItemId Id { get; set; }

		public ItemId ParentFolderId { get; set; }

		public IReadOnlyCollection<SubfolderModel> Subfolders { get; set; }

		public IReadOnlyCollection<FolderDiscModel> Discs { get; set; }
	}
}
