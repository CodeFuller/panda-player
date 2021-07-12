using System;
using System.Linq;
using PandaPlayer.Adviser.Extensions;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.RankBasedAdviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Internal
{
	internal class AdviseRankCalculator : IAdviseRankCalculator
	{
		private const double MaxRank = Double.MaxValue;

		// Disc with rating 3.0 is advised (x RatingFactorMultiplier) more often that disc with rating 2.5
		private const double RatingFactorMultiplier = 1.5;

		public double CalculateSongRank(SongModel song, PlaybacksInfo playbacksInfo)
		{
			if (!song.LastPlaybackTime.HasValue)
			{
				return MaxRank;
			}

			var factorForRating = GetFactorForRating(song.GetRatingOrDefault());
			var factorForPlaybacksAge = GetFactorForPlaybackAge(playbacksInfo.GetPlaybacksPassed(song));

			return factorForRating * factorForPlaybacksAge;
		}

		public double CalculateDiscRank(DiscModel disc, PlaybacksInfo playbacksInfo)
		{
			if (!disc.GetLastPlaybackTime().HasValue)
			{
				return MaxRank;
			}

			var playbacksPassed = playbacksInfo.GetPlaybacksPassed(disc);
			return GetFactorForAverageRating(disc.GetRating()) * GetFactorForPlaybackAge(playbacksPassed);
		}

		public double CalculateDiscGroupRank(RankedDiscGroup discGroup)
		{
			return discGroup.PlaybacksPassed == Int32.MaxValue ? MaxRank :
				GetFactorForGroupDiscsNumber(discGroup.Discs.Count(d => !d.IsDeleted)) *
				GetFactorForAverageRating(discGroup.Rating) *
				GetFactorForPlaybackAge(discGroup.PlaybacksPassed);
		}

		private static double GetFactorForGroupDiscsNumber(int discsNumber)
		{
			return Math.Sqrt(discsNumber);
		}

		private static double GetFactorForRating(RatingModel rating)
		{
			var ratingValue = rating.GetRatingValueForDiscAdviser();
			return GetFactorForAverageRating(ratingValue);
		}

		private static double GetFactorForAverageRating(double rating)
		{
			return Math.Pow(RatingFactorMultiplier, rating);
		}

		private static double GetFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}
	}
}
