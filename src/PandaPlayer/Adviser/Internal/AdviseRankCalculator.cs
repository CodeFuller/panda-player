using System;
using System.Linq;
using PandaPlayer.Adviser.Extensions;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
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

		public double CalculateAdviseSetRank(AdviseSetContent adviseSet, PlaybacksInfo playbacksInfo)
		{
			if (adviseSet.LastPlaybackTime == null)
			{
				return MaxRank;
			}

			var playbacksPassed = playbacksInfo.GetPlaybacksPassed(adviseSet);
			return GetFactorForAverageRating(adviseSet.Rating) * GetFactorForPlaybackAge(playbacksPassed);
		}

		public double CalculateAdviseGroupRank(AdviseGroupContent adviseGroup, PlaybacksInfo playbacksInfo)
		{
			var playbacksPassed = playbacksInfo.GetPlaybacksPassed(adviseGroup);

			return playbacksPassed == Int32.MaxValue ? MaxRank :
				GetFactorForAdviseGroupSize(adviseGroup.AdviseSets.Count(x => !x.IsDeleted)) *
				GetFactorForAverageRating(adviseGroup.Rating) *
				GetFactorForPlaybackAge(playbacksPassed);
		}

		private static double GetFactorForAdviseGroupSize(int adviseGroupSize)
		{
			return Math.Sqrt(adviseGroupSize);
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
