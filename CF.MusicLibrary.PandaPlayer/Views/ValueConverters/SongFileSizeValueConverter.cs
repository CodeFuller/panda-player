using System;
using System.Globalization;
using System.Windows.Data;
using CF.MusicLibrary.Common;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used from XAML")]
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
