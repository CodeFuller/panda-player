using CF.Library.Core.Exceptions;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.Core.Objects.Images
{
	public class ImageInfo
	{
		public string FileName { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public long FileSize { get; set; }

		public ImageFormatType Format { get; set; }

		public string FormatName => Format.GetDescription();

		public string GetFileNameExtension()
		{
			switch (Format)
			{
				case ImageFormatType.Jpeg:
					return "jpg";

				case ImageFormatType.Png:
					return "png";

				default:
					throw new UnexpectedEnumValueException(Format);
			}
		}
	}
}
