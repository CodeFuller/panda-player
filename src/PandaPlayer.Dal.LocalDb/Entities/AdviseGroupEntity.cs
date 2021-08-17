using System.Collections.Generic;

namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class AdviseGroupEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<FolderEntity> Folders { get; set; }

		public ICollection<DiscEntity> Discs { get; set; }
	}
}
