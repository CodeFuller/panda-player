using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MusicLibrary.Services.Diagnostic.Inconsistencies;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class InconsistencySeverityLevelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is InconsistencySeverity severity) || targetType != typeof(Brush))
			{
				return null;
			}

			switch (severity)
			{
				case InconsistencySeverity.Medium:
					return Brushes.Yellow;

				case InconsistencySeverity.High:
				default:
					return Brushes.Red;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
