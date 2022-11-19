using System.Windows.Media.Imaging;

namespace PandaPlayer.Views.ClipboardAccess
{
	internal interface IClipboardDataProvider
	{
		string GetTextData();

		BitmapFrame GetImageData();
	}
}
