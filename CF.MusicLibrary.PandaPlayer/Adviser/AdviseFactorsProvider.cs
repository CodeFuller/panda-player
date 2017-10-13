using System;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser
{
	public class AdviseFactorsProvider : IAdviseFactorsProvider
	{
		//	Disc with rating 3.0 is advised (x RatingFactorMultiplier) more often that disc with rating 2.5
		private const double RatingFactorMultiplier = 2;

		public double GetFactorForGroupDiscsNumber(int discsNumber)
		{
			return Math.Sqrt(discsNumber);
		}

		public double GetFactorForRating(Rating rating)
		{
			return GetFactorForAverageRating((double)rating);
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
