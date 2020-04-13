using System;
using System.Globalization;
using MusicLibrary.PandaPlayer.ViewModels;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
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
