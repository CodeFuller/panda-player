using System;

namespace PandaPlayer.Core.Models
{
	public class DiscImageModel : BasicModel
	{
		public DiscModel Disc { get; internal set; }

		public string TreeTitle { get; set; }

		public DiscImageType ImageType { get; set; }

		public long Size { get; set; }

		public uint Checksum { get; set; }

		public Uri ContentUri { get; set; }
	}
}
