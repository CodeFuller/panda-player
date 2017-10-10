using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CF.MusicLibrary.Common.DiscArt
{
	public sealed class ImageFacade : IImageFacade
	{
		private readonly Image image;
		private Image Image
		{
			get
			{
				if (image == null)
				{
					throw new InvalidOperationException("Image object is not set");
				}

				return image;
			}
		}

		public ImageFormat RawFormat => Image.RawFormat;

		public ImageFacade()
		{
		}

		private ImageFacade(Image image)
		{
			this.image = image;
		}

		public IImageFacade FromFile(string fileName)
		{
			return new ImageFacade(Image.FromFile(fileName));
		}

		public void Dispose()
		{
			image?.Dispose();
		}
	}
}
