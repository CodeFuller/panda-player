using System;
using System.Globalization;
using System.Windows.Data;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used from XAML")]
	internal class SongFileSizeValueConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Int32) || targetType != typeof(String))
			{
				return null;
			}

			return FileSizeFormatter.GetFormattedFileSize((int)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
