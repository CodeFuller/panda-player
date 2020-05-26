using System;

namespace MusicLibrary.Core.Models
{
	public class DiscImageModel
	{
		public ItemId Id { get; set; }

		public DiscModel Disc { get; set; }

		// TODO: Extend entity with TreeTitle property.
		public string TreeTitle { get; set; } = "cover.jpg";

		public DiscImageType ImageType { get; set; }

		public long Size { get; set; }

		public Uri ContentUri { get; set; }
	}
}
