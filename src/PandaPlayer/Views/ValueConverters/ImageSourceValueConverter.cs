using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PandaPlayer.Views.ValueConverters
{
	// https://stackoverflow.com/a/5628347/5740031
	// https://stackoverflow.com/a/29625691/5740031
	public class ImageSourceValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(ImageSource))
			{
				return DependencyProperty.UnsetValue;
			}

			string imageFileName;

			if (value is Uri imageUri)
			{
				if (!imageUri.IsFile)
				{
					throw new NotSupportedException("Displaying of non-file URIs is not supported");
				}

				imageFileName = imageUri.OriginalString;
			}
			else if (value is string imagePath)
			{
				imageFileName = imagePath;
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}

			return LoadImage(imageFileName);
		}

		private static BitmapImage LoadImage(string imageFileName)
		{
			if (!File.Exists(imageFileName))
			{
				return new BitmapImage(new Uri("pack://application:,,,/PandaPlayer;component/Views/Icons/ImageNotFound.png", UriKind.Absolute));
			}

			var image = new BitmapImage();
			using var fs = new FileStream(imageFileName, FileMode.Open, FileAccess.Read);
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
