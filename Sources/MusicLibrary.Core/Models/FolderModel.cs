using System.Collections.Generic;

namespace MusicLibrary.Core.Models
{
	public class FolderModel : ShallowFolderModel
	{
		public ShallowFolderModel ParentFolder { get; set; }

		public IReadOnlyCollection<ShallowFolderModel> Subfolders { get; set; }

		public IReadOnlyCollection<DiscModel> Discs { get; set; }
	}
}
