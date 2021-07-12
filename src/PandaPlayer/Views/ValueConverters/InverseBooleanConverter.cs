using System;
using System.Globalization;
using System.Windows.Data;

namespace PandaPlayer.Views.ValueConverters
{
	// http://stackoverflow.com/a/1039681/5740031
	[ValueConversion(typeof(bool), typeof(bool))]
	internal class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(bool))
			{
				throw new InvalidOperationException("The target must be a boolean");
			}

			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
