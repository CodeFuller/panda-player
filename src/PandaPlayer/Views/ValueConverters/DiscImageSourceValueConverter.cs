using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PandaPlayer.ViewModels.DiscImages;
using PandaPlayer.Views.Helpers;

namespace PandaPlayer.Views.ValueConverters
{
	public class DiscImageSourceValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not DiscImageSource imageSource || targetType != typeof(ImageSource))
			{
				return DependencyProperty.UnsetValue;
			}

			if (imageSource.IsImageForDeletedDisc)
			{
				return new BitmapImage(new Uri("pack://application:,,,/PandaPlayer;component/Views/Icons/Deleted.png", UriKind.Absolute));
			}

			if (imageSource.IsMissingImage)
			{
				return new BitmapImage(new Uri("pack://application:,,,/PandaPlayer;component/Views/Icons/Image-Not-Found.png", UriKind.Absolute));
			}

			return BitmapImageLoader.LoadImageFromFile(imageSource.FilePath);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
