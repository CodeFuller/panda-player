using System;

namespace PandaPlayer.Shared.Images
{
	internal interface IImageFacade : IDisposable
	{
		int Width { get; }

		int Height { get; }

		ImageFormatType Format { get; }

		IImageFacade FromFile(string fileName);
	}
}
