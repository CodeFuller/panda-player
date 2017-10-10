using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;

namespace CF.MusicLibrary.Common.DiscArt
{
	public class DiscArtValidator : IDiscArtValidator
	{
		private static readonly ImageFormat[] SupportedDiscCoverImageFormats =
		{
			ImageFormat.Jpeg,
			ImageFormat.Png,
		};

		private const int MinWidthAndHeight = 300;

		private const int MaxWidthAndHeight = 5000;

		private const long MaxFileSize = 10 * 1024 * 1024;

		private readonly IFileSystemFacade fileSystemFacade;

		public DiscArtValidator(IFileSystemFacade fileSystemFacade)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.fileSystemFacade = fileSystemFacade;
		}

		public DiscArtImageInfo GetImageInfo(string imageFileName)
		{
			using (Image image = Image.FromFile(imageFileName))
			{
				return new DiscArtImageInfo
				{
					Width = image.Width,
					Height = image.Height,
					FileSize = fileSystemFacade.GetFileSize(imageFileName),
					Format = image.RawFormat,
				};
			}
		}

		public DiscArtValidationResults ValidateDiscCoverImage(string imageFileName)
		{
			return ValidateDiscCoverImage(GetImageInfo(imageFileName));
		}

		public DiscArtValidationResults ValidateDiscCoverImage(DiscArtImageInfo imageInfo)
		{
			DiscArtValidationResults validationResults = DiscArtValidationResults.None;

			if (imageInfo.Width < MinWidthAndHeight || imageInfo.Height < MinWidthAndHeight)
			{
				validationResults |= DiscArtValidationResults.ImageIsTooSmall;
			}

			if (imageInfo.Width > MaxWidthAndHeight || imageInfo.Height > MaxWidthAndHeight)
			{
				validationResults |= DiscArtValidationResults.ImageIsTooBig;
			}

			if (imageInfo.FileSize > MaxFileSize)
			{
				validationResults |= DiscArtValidationResults.FileSizeIsTooBig;
			}

			if (SupportedDiscCoverImageFormats.All(format => !format.Equals(imageInfo.Format)))
			{
				validationResults |= DiscArtValidationResults.UnsupportedFormat;
			}

			return validationResults == DiscArtValidationResults.None ? DiscArtValidationResults.ImageIsOk : validationResults;
		}

		public IEnumerable<string> GetValidationResultsHints(DiscArtValidationResults results)
		{
			foreach (var enumValue in Enum.GetValues(typeof(DiscArtValidationResults)).Cast<DiscArtValidationResults>())
			{
				if ((results & enumValue) != 0)
				{
					yield return enumValue.GetDescription();
				}
			}
		}
	}
}
