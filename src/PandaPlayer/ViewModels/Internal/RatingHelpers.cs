using System.Collections.Generic;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Internal
{
	internal static class RatingHelpers
	{
		public static RatingModel DefaultValueProposedForAssignment => RatingModel.R5;

		public static IEnumerable<RatingModel> AllRatingValues
		{
			get
			{
				yield return RatingModel.R1;
				yield return RatingModel.R2;
				yield return RatingModel.R3;
				yield return RatingModel.R4;
				yield return RatingModel.R5;
				yield return RatingModel.R6;
				yield return RatingModel.R7;
				yield return RatingModel.R8;
				yield return RatingModel.R9;
				yield return RatingModel.R10;
			}
		}
	}
}
