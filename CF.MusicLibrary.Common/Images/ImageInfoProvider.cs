using System;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Common.Images
{
	public class ImageInfoProvider : IImageInfoProvider
	{
		private readonly IImageFacade imageFacade;
		private readonly IFileSystemFacade fileSystemFacade;

		public ImageInfoProvider(IImageFacade imageFacade, IFileSystemFacade fileSystemFacade)
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

		public ImageInfo GetImageInfo(string imageFileName)
		{
			var image = imageFacade.FromFile(imageFileName);
			return new ImageInfo
			{
				FileName = imageFileName,
				Width = image.Width,
				Height = image.Height,
				FileSize = fileSystemFacade.GetFileSize(imageFileName),
				Format = image.Format,
			};
		}
	}
}
