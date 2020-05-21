using System;
using System.Globalization;
using System.Windows.Data;
using CF.Library.Core.Extensions;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class SongLastPlaybackValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return "N/A";
			}

			if (!(value is DateTimeOffset dt) || targetType != typeof(String))
			{
				return null;
			}

			string dayPart;
			if (dt.Date == DateTimeOffset.Now.Date)
			{
				dayPart = "Today";
			}
			else if (dt.Date == DateTimeOffset.Now.AddDays(-1).Date)
			{
				dayPart = "Yesterday";
			}
			else
			{
				dayPart = dt.ToString("yyyy.MM.dd", CultureInfo.CurrentCulture);
			}

			return FormattableStringExtensions.Current($"{dayPart} {dt:HH:mm}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
