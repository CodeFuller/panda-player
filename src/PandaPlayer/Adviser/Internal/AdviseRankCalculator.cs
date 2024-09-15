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
			return GetFactorForAverageRating(GetRating(adviseSet)) * GetFactorForPlaybackAge(playbacksPassed);
		}

		public double CalculateAdviseGroupRank(AdviseGroupContent adviseGroup, PlaybacksInfo playbacksInfo)
		{
			var playbacksPassed = playbacksInfo.GetPlaybacksPassed(adviseGroup);

			return playbacksPassed == Int32.MaxValue ? MaxRank :
				GetFactorForAdviseGroupSize(adviseGroup.AdviseSets.Count(x => !x.IsDeleted)) *
				GetFactorForAverageRating(GetRating(adviseGroup)) *
				GetFactorForPlaybackAge(playbacksPassed);
		}

		private static double GetFactorForAdviseGroupSize(int adviseGroupSize)
		{
			return Math.Min(Math.Sqrt(adviseGroupSize), 3);
		}

		private static double GetFactorForRating(RatingModel rating)
		{
			var ratingValue = rating.GetRatingValueForAdviser();
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

		private static double GetRating(AdviseGroupContent adviseGroupContent)
		{
			return adviseGroupContent.AdviseSets
				.Where(x => !x.IsDeleted)
				.Select(GetRating)
				.Average();
		}

		private static double GetRating(AdviseSetContent adviseSetContent)
		{
			// Below check was added just in case.
			// It should not happen that advise set contains both active and deleted discs.
			// If all discs in advise set are deleted, then the whole advise set is considered as deleted and should be filtered by the caller.
			if (adviseSetContent.Discs.Any(x => x.IsDeleted))
			{
				throw new InvalidOperationException("Advise set contains both active and deleted discs");
			}

			return adviseSetContent.Discs
				.Select(x => x.GetRating())
				.Average();
		}
	}
}
