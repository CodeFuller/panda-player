using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PandaPlayer.Core.Models;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views.ValueConverters
{
	internal class RatingToImageNameValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not RatingModel rating || targetType != typeof(ImageSource))
			{
				return null;
			}

			return rating.ToRatingImageFileName();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
