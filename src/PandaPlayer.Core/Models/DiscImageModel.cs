using System;

namespace PandaPlayer.Core.Models
{
	public class DiscImageModel
	{
		public ItemId Id { get; set; }

		public DiscModel Disc { get; set; }

		public string TreeTitle { get; set; }

		public DiscImageType ImageType { get; set; }

		public long Size { get; set; }

		public uint Checksum { get; set; }

		public Uri ContentUri { get; set; }
	}
}
