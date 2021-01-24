using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class SongsTotalDurationValueConverter : IValueConverter
	{
		private const int SecondsInOneMinute = 60;
		private const int SecondsInOneHour = 60 * SecondsInOneMinute;
		private const int SecondsInOneDay = 24 * SecondsInOneHour;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TimeSpan) || (targetType != typeof(String) && targetType != typeof(Object)))
			{
				return null;
			}

			double seconds = ((TimeSpan)value).TotalSeconds;

			if (seconds < SecondsInOneMinute)
			{
				return $"{seconds:F0} seconds";
			}

			if (seconds < SecondsInOneHour)
			{
				return $"{seconds / SecondsInOneMinute:F1} minutes";
			}

			if (seconds < SecondsInOneDay)
			{
				return $"{seconds / SecondsInOneHour:F1} hours";
			}

			return $"{seconds / SecondsInOneDay:F1} days";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
