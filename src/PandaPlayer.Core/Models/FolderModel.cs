using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.Core.Models
{
	public class FolderModel : ShallowFolderModel
	{
		public ShallowFolderModel ParentFolder { get; set; }

		public IReadOnlyCollection<ShallowFolderModel> Subfolders { get; set; }

		public IReadOnlyCollection<DiscModel> Discs { get; set; }

		public bool HasContent => Subfolders.Any(f => !f.IsDeleted) || Discs.Any(d => !d.IsDeleted);
	}
}
