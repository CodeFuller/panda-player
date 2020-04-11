namespace CF.MusicLibrary.Core.Objects.Images
{
	public class DiscImage : ItemImage
	{
		public int? DiscId { get; set; }

		public Disc Disc { get; set; }

		public DiscImageType ImageType { get; set; }
	}
}
