using System;
using System.Globalization;
using System.Windows.Data;

namespace CF.MusicLibrary.PandaPlayer.Views.ValueConverters
{
	public class BooleanAndConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			foreach (object value in values)
			{
				var boolValue = value as bool?;
				if (boolValue.HasValue && !boolValue.Value)
				{
					return false;
				}
			}

			return true;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
