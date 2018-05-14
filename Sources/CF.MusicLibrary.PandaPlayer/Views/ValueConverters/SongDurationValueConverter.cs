using System;
using System.Globalization;
using System.Windows.Data;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class SongDurationValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TimeSpan) || (targetType != typeof(String) && targetType != typeof(Object)))
			{
				return null;
			}

			var ts = (TimeSpan)value;
			return (int)ts.TotalHours > 0 ? FormattableStringExtensions.Current($"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}") : FormattableStringExtensions.Current($"{ts.Minutes}:{ts.Seconds:D2}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
