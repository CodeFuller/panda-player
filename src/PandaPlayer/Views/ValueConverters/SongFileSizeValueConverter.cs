using System;
using System.Globalization;
using System.Windows.Data;
using PandaPlayer.Shared;

namespace PandaPlayer.Views.ValueConverters
{
	internal class SongFileSizeValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(String) && targetType != typeof(Object))
			{
				return null;
			}

			if (value is long longValue)
			{
				return FileSizeFormatter.GetFormattedFileSize(longValue);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
