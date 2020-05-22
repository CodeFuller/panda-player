using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	// https://stackoverflow.com/a/5628347/5740031
	// https://stackoverflow.com/a/29625691/5740031
	public class ImageSourceValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Uri imageUri) || targetType != typeof(ImageSource))
			{
				return DependencyProperty.UnsetValue;
			}

			if (!imageUri.IsFile)
			{
				throw new NotSupportedException("Displaying of non-file URIs is not supported");
			}

			var image = new BitmapImage();
			using var fs = new FileStream(imageUri.OriginalString, FileMode.Open, FileAccess.Read);
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.StreamSource = fs;
			image.EndInit();

			return image;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
