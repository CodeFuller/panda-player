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
			this.imageFacade = imageFacade ?? throw new ArgumentNullException(nameof(imageFacade));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
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
