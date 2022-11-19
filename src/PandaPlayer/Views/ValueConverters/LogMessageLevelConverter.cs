using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.Logging;

namespace PandaPlayer.Views.ValueConverters
{
	internal class LogMessageLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is LogLevel logLevel) || targetType != typeof(Brush))
			{
				return null;
			}

			switch (logLevel)
			{
				case LogLevel.Critical:
				case LogLevel.Error:
					return Brushes.Red;

				case LogLevel.Warning:
					return Brushes.Yellow;

				case LogLevel.Debug:
				case LogLevel.Trace:
					return Brushes.Gray;

				case LogLevel.Information:
				default:
					return Brushes.White;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
