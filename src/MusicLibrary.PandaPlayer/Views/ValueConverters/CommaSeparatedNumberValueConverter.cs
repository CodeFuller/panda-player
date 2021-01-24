using System;
using System.Globalization;
using System.Windows.Data;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	public class CommaSeparatedNumberValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Int32) || (targetType != typeof(String) && targetType != typeof(Object)))
			{
				return null;
			}

			return Current($"{(int)value:n0}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
