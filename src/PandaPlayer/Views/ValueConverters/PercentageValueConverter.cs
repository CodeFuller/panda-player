using System;
using System.Globalization;
using System.Windows.Data;

namespace PandaPlayer.Views.ValueConverters
{
	public class PercentageValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is double) || (targetType != typeof(String) && targetType != typeof(Object)))
			{
				return null;
			}

			return $"{100 * (double)value:F1} %";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
