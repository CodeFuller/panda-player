using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Common.DiscArt
{
	public class DiscArtFileStorage : IDiscArtFileStorage
	{
		private static readonly IReadOnlyDictionary<ImageFormat, string> PossibleDiscCoverFileNames = new Dictionary< ImageFormat, string>()
		{
			{ ImageFormat.Jpeg, "cover.jpg" },
			{ ImageFormat.Png, "cover.png" },
		};

		private readonly IImageFacade imageFacade;
		private readonly IFileSystemFacade fileSystemFacade;

		public DiscArtFileStorage(IImageFacade imageFacade, IFileSystemFacade fileSystemFacade)
		{
			if (imageFacade == null)
			{
				throw new ArgumentNullException(nameof(imageFacade));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.imageFacade = imageFacade;
			this.fileSystemFacade = fileSystemFacade;
		}

		public string GetDiscCoverImageFileName(string discDirectory)
		{
			var discCoverImageFileNames = GetPossibleDiscCoverImageFileNames(discDirectory)
				.Where(fileName => fileSystemFacade.FileExists(fileName)).ToList();

			if (discCoverImageFileNames.Count > 1)
			{
				throw new InvalidOperationException(Current($"Multiple cover images found in directory '{discDirectory}'"));
			}

			return discCoverImageFileNames.SingleOrDefault();
		}

		private static IEnumerable<string> GetPossibleDiscCoverImageFileNames(string discDirectory)
		{
			return PossibleDiscCoverFileNames.Values.Select(name => Path.Combine(discDirectory, name));
		}

		public bool IsCoverImageFile(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			return PossibleDiscCoverFileNames.Values.Any(name => String.Equals(name, fileName, StringComparison.OrdinalIgnoreCase));
		}

		public void StoreDiscCoverImage(string discDirectory, string sourceCoverImageFileName)
		{
			string targetDiscCoverImageFileName = Path.Combine(discDirectory, GetTargetDiscCoverImageFileName(sourceCoverImageFileName));

			var existingDiscCoverImageFileName = GetDiscCoverImageFileName(discDirectory);
			if (existingDiscCoverImageFileName != null)
			{
				fileSystemFacade.ClearReadOnlyAttribute(existingDiscCoverImageFileName);
				fileSystemFacade.DeleteFile(existingDiscCoverImageFileName);
			}

			fileSystemFacade.CopyFile(sourceCoverImageFileName, targetDiscCoverImageFileName);
			fileSystemFacade.SetReadOnlyAttribute(targetDiscCoverImageFileName);
		}

		private string GetTargetDiscCoverImageFileName(string sourceCoverImageFileName)
		{
			using (var image = imageFacade.FromFile(sourceCoverImageFileName))
			{
				string targetFileName;
				if (!PossibleDiscCoverFileNames.TryGetValue(image.RawFormat, out targetFileName))
				{
					throw new InvalidOperationException(Current($"Disc covers of '{image.RawFormat}' format are not supported"));
				}

				return targetFileName;
			}
		}
	}
}
