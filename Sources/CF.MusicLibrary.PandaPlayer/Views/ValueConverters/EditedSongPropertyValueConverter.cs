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

	public class SongPropertyNullableShortValueConverter : SongPropertyValueConverter<short?>
	{
		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = value as string;
			if (stringValue == null || targetType != typeof(EditedSongProperty<short?>))
			{
				return null;
			}

			return new EditedSongProperty<short?>(stringValue.Length == 0 ? null : (short?)short.Parse(stringValue, culture));
		}
	}
}
