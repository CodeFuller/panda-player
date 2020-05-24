using System;

namespace MusicLibrary.Core.Models
{
	public class DiscImageModel
	{
		public ItemId Id { get; set; }

		public DiscImageType ImageType { get; set; }

		public Uri Uri { get; set; }

		public long Size { get; set; }
	}
}
