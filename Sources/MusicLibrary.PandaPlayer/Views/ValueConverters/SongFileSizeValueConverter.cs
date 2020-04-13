using System;
using System.Globalization;
using System.Windows.Data;
using MusicLibrary.Common;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class SongFileSizeValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(String) && targetType != typeof(Object))
			{
				return null;
			}

			if (value is Int32)
			{
				return FileSizeFormatter.GetFormattedFileSize((int)value);
			}

			if (value is Int64)
			{
				return FileSizeFormatter.GetFormattedFileSize((long)value);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
