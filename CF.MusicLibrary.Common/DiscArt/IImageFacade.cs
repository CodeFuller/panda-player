using System;
using System.Drawing.Imaging;

namespace CF.MusicLibrary.Common.DiscArt
{
	/// <summary>
	/// Facade interface for System.Drawing.Image.
	/// </summary>
	public interface IImageFacade : IDisposable
	{
		/// <summary>
		/// Gets the Image file format.
		/// </summary>
		ImageFormat RawFormat { get; }

		/// <summary>
		/// Creates instance of IImageFacade for the specified file.
		/// </summary>
		IImageFacade FromFile(string fileName);
	}
}
