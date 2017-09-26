using System.Windows.Media.Imaging;

namespace CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardDataProvider
	{
		string GetTextData();

		BitmapFrame GetImageData();
	}
}
