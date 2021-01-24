using System.Windows.Media.Imaging;

namespace MusicLibrary.PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardDataProvider
	{
		string GetTextData();

		BitmapFrame GetImageData();
	}
}
