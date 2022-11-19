using System.IO;
using System.Windows.Media.Imaging;

namespace PandaPlayer.Views.Helpers
{
	internal static class BitmapImageLoader
	{
		public static BitmapImage LoadImageFromFile(string imagePath)
		{
			var image = new BitmapImage();
			using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = fs;
			image.EndInit();

			return image;
		}
	}
}
