using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PandaPlayer.DiscAdder.Views.ValueConverters
{
	public class DataReadinessToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not bool boolValue || targetType != typeof(Visibility))
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
