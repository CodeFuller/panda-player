using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CF.MusicLibrary.Core.Objects;
using static System.FormattableString;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class RatingToImageNameValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Rating) || targetType != typeof(ImageSource))
			{
				return null;
			}

			return Invariant($"/Views/Icons/Ratings/Rating{(int)value:D2}.png");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
