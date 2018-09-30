using System;
using System.Globalization;
using System.Windows.Data;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	public abstract class SongPropertyValueConverter<T> : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var propertyValue = value as EditedSongProperty<T>;
			return propertyValue?.ToString();
		}

		public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}
}
