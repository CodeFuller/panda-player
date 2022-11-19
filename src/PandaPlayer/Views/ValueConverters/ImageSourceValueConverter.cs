using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PandaPlayer.Views.Helpers;

namespace PandaPlayer.Views.ValueConverters
{
	// https://stackoverflow.com/a/5628347/5740031
	// https://stackoverflow.com/a/29625691/5740031
	public class ImageSourceValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not String imagePath || targetType != typeof(ImageSource))
			{
				return DependencyProperty.UnsetValue;
			}

			return BitmapImageLoader.LoadImageFromFile(imagePath);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
