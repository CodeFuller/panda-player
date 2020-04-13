using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	public class BooleanToVisibilityValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? isVisibleValue = value as bool?;
			if (isVisibleValue == null || targetType != typeof(Visibility))
			{
				return null;
			}

			return isVisibleValue.Value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
