using System.Collections.Generic;

namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class AdviseSetEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<DiscEntity> Discs { get; set; }
	}
}
