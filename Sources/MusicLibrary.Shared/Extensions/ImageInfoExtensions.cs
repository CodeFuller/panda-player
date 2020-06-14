using System.IO;
using CF.Library.Core.Exceptions;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.Shared.Extensions
{
	public static class ImageInfoExtensions
	{
		public static string GetDiscCoverImageTreeTitle(this ImageInfo imageInfo)
		{
			return Path.ChangeExtension("cover", GetImageFileNameExtension(imageInfo));
		}

		public static string GetImageFileNameExtension(ImageInfo imageInfo)
		{
			switch (imageInfo.Format)
			{
				case ImageFormatType.Jpeg:
					return "jpg";

				case ImageFormatType.Png:
					return "png";

				default:
					throw new UnexpectedEnumValueException(imageInfo.Format);
			}
		}
	}
}
