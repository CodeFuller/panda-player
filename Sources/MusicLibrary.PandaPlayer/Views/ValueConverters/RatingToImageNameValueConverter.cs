using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MusicLibrary.Core.Models;
using static System.FormattableString;

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

			return Invariant($"/Views/Icons/Ratings/{GetRatingImageFileName(rating)}");
		}

		private static string GetRatingImageFileName(RatingModel rating)
		{
			if (rating == RatingModel.R1)
			{
				return "Rating01.png";
			}

			if (rating == RatingModel.R2)
			{
				return "Rating02.png";
			}

			if (rating == RatingModel.R3)
			{
				return "Rating03.png";
			}

			if (rating == RatingModel.R4)
			{
				return "Rating04.png";
			}

			if (rating == RatingModel.R5)
			{
				return "Rating05.png";
			}

			if (rating == RatingModel.R6)
			{
				return "Rating06.png";
			}

			if (rating == RatingModel.R7)
			{
				return "Rating07.png";
			}

			if (rating == RatingModel.R8)
			{
				return "Rating08.png";
			}

			if (rating == RatingModel.R9)
			{
				return "Rating09.png";
			}

			if (rating == RatingModel.R10)
			{
				return "Rating10.png";
			}

			throw new InvalidOperationException($"Unexpected value for the rating: {rating}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
