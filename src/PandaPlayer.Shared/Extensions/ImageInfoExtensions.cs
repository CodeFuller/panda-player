using System;
using System.IO;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.Shared.Extensions
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
					throw new NotSupportedException($"Image format {imageInfo.Format} is not supported");
			}
		}
	}
}
