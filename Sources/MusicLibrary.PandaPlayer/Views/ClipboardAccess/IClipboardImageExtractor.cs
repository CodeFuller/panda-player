using System.Windows.Media.Imaging;

namespace MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardImageExtractor
	{
		BitmapFrame GetImageFromClipboard();
	}
}
