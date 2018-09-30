using System;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Common.Images
{
	/// <summary>
	/// Facade interface for System.Drawing.Image.
	/// </summary>
	public interface IImageFacade : IDisposable
	{
		/// <summary>
		/// Gets the width, in pixels, of this System.Drawing.Image.
		/// </summary>
		int Width { get; }

		/// <summary>
		/// Gets the height, in pixels, of this System.Drawing.Image.
		/// </summary>
		int Height { get; }

		/// <summary>
		/// Gets the file format of this System.Drawing.Image.
		/// </summary>
		ImageFormatType Format { get; }

		/// <summary>
		/// Creates instance of IImageFacade for the specified file.
		/// </summary>
		IImageFacade FromFile(string fileName);
	}
}
