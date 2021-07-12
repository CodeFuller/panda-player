using System;
using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Extensions
{
	internal static class RatingModelExtensions
	{
		private static readonly Dictionary<RatingModel, double> RatingValues = new()
		{
			{ RatingModel.R1, 1 },
			{ RatingModel.R2, 2 },
			{ RatingModel.R3, 3 },
			{ RatingModel.R4, 4 },
			{ RatingModel.R5, 5 },
			{ RatingModel.R6, 6 },
			{ RatingModel.R7, 7 },
			{ RatingModel.R8, 8 },
			{ RatingModel.R9, 9 },
			{ RatingModel.R10, 10 },
		};

		public static double GetRatingValueForDiscAdviser(this RatingModel rating)
		{
			if (RatingValues.TryGetValue(rating, out var value))
			{
				return value;
			}

			throw new InvalidOperationException($"Unexpected rating value {rating}");
		}
	}
}
