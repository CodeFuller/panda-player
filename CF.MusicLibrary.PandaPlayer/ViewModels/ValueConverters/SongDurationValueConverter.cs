using System;
using System.Globalization;
using System.Windows.Data;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.ValueConverters
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used from XAML")]
	internal class SongDurationValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TimeSpan) || (targetType != typeof(String) && targetType != typeof(Object)))
			{
				return null;
			}

			var ts = (TimeSpan)value;
			return (int)ts.TotalHours > 0 ? Current($"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}") : Current($"{ts.Minutes}:{ts.Seconds:D2}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
