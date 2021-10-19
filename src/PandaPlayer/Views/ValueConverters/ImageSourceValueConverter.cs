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

			return value switch
			{
				Uri imageUri => LoadImage(imageUri),
				string imagePath => LoadImageFromFile(imagePath),
				_ => DependencyProperty.UnsetValue,
			};
		}

		private static BitmapImage LoadImage(Uri imageUri)
		{
			if (imageUri.Scheme == "pack")
			{
				return new BitmapImage(imageUri);
			}

			if (imageUri.IsFile)
			{
				return LoadImageFromFile(imageUri.OriginalString);
			}

			throw new NotSupportedException($"Image URL is not supported: '{imageUri}'");
		}

		private static BitmapImage LoadImageFromFile(string imagePath)
		{
			var image = new BitmapImage();
			using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
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
