using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Views.ValueConverters
{
	internal class RatingToImageNameValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is RatingModel rating) || targetType != typeof(ImageSource))
			{
				return null;
			}

			return $"/Views/Icons/Ratings/{GetRatingImageFileName(rating)}";
		}

		private static string GetRatingImageFileName(RatingModel rating)
		{
			return rating switch
			{
				RatingModel.R1 => "Rating01.png",
				RatingModel.R2 => "Rating02.png",
				RatingModel.R3 => "Rating03.png",
				RatingModel.R4 => "Rating04.png",
				RatingModel.R5 => "Rating05.png",
				RatingModel.R6 => "Rating06.png",
				RatingModel.R7 => "Rating07.png",
				RatingModel.R8 => "Rating08.png",
				RatingModel.R9 => "Rating09.png",
				RatingModel.R10 => "Rating10.png",
				_ => throw new InvalidOperationException($"Unexpected value for the rating: {rating}"),
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
