namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class DiscImageEntity
	{
		public int Id { get; set; }

		public int? DiscId { get; set; }

		public DiscEntity Disc { get; set; }

		public DiscImageType ImageType { get; set; }

		public int FileSize { get; set; }

		public int? Checksum { get; set; }
	}
}
