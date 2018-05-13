﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Common.Images
{
	public class DiscImageValidator : IDiscImageValidator
	{
		private class SupportedImageFormat
		{
			public ImageFormatType ImageFormat { get; }

			public string FileNameExtension { get; }

			public SupportedImageFormat(ImageFormatType imageFormat, string fileNameExtension)
			{
				ImageFormat = imageFormat;
				FileNameExtension = fileNameExtension;
			}
		}

		private static readonly SupportedImageFormat[] SupportedDiscCoverImageFormats =
		{
			new SupportedImageFormat(ImageFormatType.Jpeg, ".jpg"),
			new SupportedImageFormat(ImageFormatType.Png, ".png"),
		};

		private const int MinWidthAndHeight = 300;

		private const int MaxWidthAndHeight = 5000;

		private const long MaxFileSize = 10 * 1024 * 1024;

		public ImageValidationResults ValidateDiscCoverImage(ImageInfo imageInfo)
		{
			ImageValidationResults validationResults = ImageValidationResults.None;

			if (imageInfo.Width < MinWidthAndHeight || imageInfo.Height < MinWidthAndHeight)
			{
				validationResults |= ImageValidationResults.ImageIsTooSmall;
			}

			if (imageInfo.Width > MaxWidthAndHeight || imageInfo.Height > MaxWidthAndHeight)
			{
				validationResults |= ImageValidationResults.ImageIsTooBig;
			}

			if (imageInfo.FileSize > MaxFileSize)
			{
				validationResults |= ImageValidationResults.FileSizeIsTooBig;
			}

			if (SupportedDiscCoverImageFormats.All(supportedFormat => !supportedFormat.ImageFormat.Equals(imageInfo.Format)))
			{
				validationResults |= ImageValidationResults.UnsupportedFormat;
			}

			return validationResults == ImageValidationResults.None ? ImageValidationResults.ImageIsOk : validationResults;
		}

		public IEnumerable<string> GetValidationResultsHints(ImageValidationResults results)
		{
			foreach (var enumValue in Enum.GetValues(typeof(ImageValidationResults)).Cast<ImageValidationResults>())
			{
				if ((results & enumValue) != 0)
				{
					yield return enumValue.GetDescription();
				}
			}
		}

		public bool IsSupportedFileFormat(string fileName)
		{
			var fileExtension = Path.GetExtension(fileName);
			return SupportedDiscCoverImageFormats.Any(supportedFormat => String.Equals(fileExtension, supportedFormat.FileNameExtension, StringComparison.OrdinalIgnoreCase));
		}
	}
}