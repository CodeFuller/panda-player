using System;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;

namespace MusicLibrary.PandaPlayer.Adviser.Internal
{
	internal class AdviseFactorsProvider : IAdviseFactorsProvider
	{
		// Disc with rating 3.0 is advised (x RatingFactorMultiplier) more often that disc with rating 2.5
		private const double RatingFactorMultiplier = 1.5;

		public double GetFactorForGroupDiscsNumber(int discsNumber)
		{
			return Math.Sqrt(discsNumber);
		}

		public double GetFactorForRating(RatingModel rating)
		{
			var ratingValue = rating.GetRatingValueForDiscAdviser();
			return GetFactorForAverageRating(ratingValue);
		}

		public double GetFactorForAverageRating(double rating)
		{
			return Math.Pow(RatingFactorMultiplier, rating);
		}

		public double GetFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}
	}
}
