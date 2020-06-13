using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicLibrary.DiscAdder.Views.ValueConverters
{
	public class DataReadinessToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool boolValue) || targetType != typeof(Visibility))
			{
				return null;
			}

			return boolValue ? Visibility.Hidden : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
