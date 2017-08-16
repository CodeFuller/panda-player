using System;
using System.Globalization;
using System.Windows.Data;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.ValueConverters
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used from XAML")]
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

			return Current($"{dayPart} {dt:HH:mm}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
