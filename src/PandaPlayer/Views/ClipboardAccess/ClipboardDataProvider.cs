using System.Windows;
using System.Windows.Media.Imaging;

namespace PandaPlayer.Views.ClipboardAccess
{
	internal class ClipboardDataProvider : IClipboardDataProvider
	{
		private readonly ClipboardImageExtractor clipboardImageExtractor = new ClipboardImageExtractor();

		public string GetTextData()
		{
			return Clipboard.ContainsText() ? Clipboard.GetText() : null;
		}

		public BitmapFrame GetImageData()
		{
			return Clipboard.ContainsImage() ? clipboardImageExtractor.GetImageFromClipboard() : null;
		}
	}
}
