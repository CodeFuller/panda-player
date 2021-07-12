using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PandaPlayer.Shared.Images
{
	internal sealed class ImageFacade : IImageFacade
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

		public int Width => Image.Width;

		public int Height => Image.Height;

		public ImageFormatType Format
		{
			get
			{
				if (Image.RawFormat.Equals(ImageFormat.Jpeg))
				{
					return ImageFormatType.Jpeg;
				}

				if (Image.RawFormat.Equals(ImageFormat.Png))
				{
					return ImageFormatType.Png;
				}

				return ImageFormatType.Unsupported;
			}
		}

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
