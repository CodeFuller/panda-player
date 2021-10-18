using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Views.Extensions
{
	public static class RatingModelExtensions
	{
		public static string ToRatingImageFileName(this RatingModel rating)
		{
			return $"/Views/Icons/Ratings/{GetRelativeImageFileName(rating)}";
		}

		private static string GetRelativeImageFileName(RatingModel rating)
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
	}
}
