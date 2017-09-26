using System.Drawing;
using System.Drawing.Imaging;

namespace CF.MusicLibrary.Universal.DiscArt
{
	public class DiscArtImageInfo
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public long FileSize { get; set; }

		public ImageFormat Format { get; set; }

		public string FormatName => new ImageFormatConverter().ConvertToString(Format);
	}
}
