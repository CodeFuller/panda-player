using PandaPlayer.Shared.Extensions;

namespace PandaPlayer.Shared.Images
{
	public class ImageInfo
	{
		public string FileName { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public long FileSize { get; set; }

		public ImageFormatType Format { get; set; }

		public string FormatName => Format.GetDescription();
	}
}
