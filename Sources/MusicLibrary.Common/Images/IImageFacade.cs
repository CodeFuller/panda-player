using System;

namespace MusicLibrary.Common.Images
{
	internal interface IImageFacade : IDisposable
	{
		int Width { get; }

		int Height { get; }

		ImageFormatType Format { get; }

		IImageFacade FromFile(string fileName);
	}
}
