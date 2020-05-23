using System;

namespace MusicLibrary.Logic.Models
{
	public class DiscImageModel
	{
		public ItemId Id { get; set; }

		public DiscImageType ImageType { get; set; }

		public Uri Uri { get; set; }

		public long Size { get; set; }
	}
}
