using System.Windows.Media.Imaging;

namespace CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	interface IClipboardImageExtractor
	{
		BitmapFrame GetImageFromClipboard();
	}
}
