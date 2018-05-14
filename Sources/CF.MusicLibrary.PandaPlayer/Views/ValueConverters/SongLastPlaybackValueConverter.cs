using System;
using System.Globalization;
using System.Windows.Data;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class SongLastPlaybackValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return "N/A";
			}

			if (!(value is DateTime) || targetType != typeof(String))
			{
				return null;
			}

			var dt = (DateTime)value;

			string dayPart;
			if (dt.Date == DateTime.Today)
			{
				dayPart = "Today";
			}
			else if (dt.Date == DateTime.Today.AddDays(-1))
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
