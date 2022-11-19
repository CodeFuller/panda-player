using System.Windows.Media.Imaging;

namespace PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardImageExtractor
	{
		BitmapFrame GetImageFromClipboard();
	}
}
