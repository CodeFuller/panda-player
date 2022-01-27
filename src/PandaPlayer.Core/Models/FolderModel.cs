using System.Collections.Generic;
using System.Linq;

namespace PandaPlayer.Core.Models
{
	public class FolderModel : ShallowFolderModel
	{
		private List<ShallowFolderModel> subfolders = new();

		private List<DiscModel> discs = new();

		public ShallowFolderModel ParentFolder { get; set; }

		public IReadOnlyCollection<ShallowFolderModel> Subfolders
		{
			get => subfolders;
			set => subfolders = new List<ShallowFolderModel>(value);
		}

		public IReadOnlyCollection<DiscModel> Discs
		{
			get => discs;
			set => discs = new List<DiscModel>(value);
		}

		public bool HasContent => Subfolders.Any(f => !f.IsDeleted) || Discs.Any(d => !d.IsDeleted);

		public void AddSubfolder(ShallowFolderModel subfolder)
		{
			subfolders.Add(subfolder);
		}

		public void AddDisc(DiscModel disc)
		{
			discs.Add(disc);
			disc.Folder = this;
		}
	}
}
