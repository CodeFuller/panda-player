using System.Collections.Generic;

namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class DiscEntity
	{
		public int Id { get; set; }

		public int FolderId { get; set; }

		public int? Year { get; set; }

		public FolderEntity Folder { get; set; }

		public int? AdviseGroupId { get; set; }

		public AdviseGroupEntity AdviseGroup { get; set; }

		public int? AdviseSetId { get; set; }

		public AdviseSetEntity AdviseSet { get; set; }

		public int? AdviseSetOrder { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		public string AlbumTitle { get; set; }

		public IReadOnlyCollection<SongEntity> Songs { get; set; }

		public ICollection<DiscImageEntity> Images { get; set; }
	}
}
