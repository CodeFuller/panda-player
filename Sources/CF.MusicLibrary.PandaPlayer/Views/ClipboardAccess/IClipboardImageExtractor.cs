using System.Windows.Media.Imaging;

namespace CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardImageExtractor
	{
		BitmapFrame GetImageFromClipboard();
	}
}
