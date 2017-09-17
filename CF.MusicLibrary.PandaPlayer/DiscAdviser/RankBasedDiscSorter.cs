using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Universal;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public class RankBasedDiscSorter : IDiscGroupSorter
	{
		//	Disc with rating 3.0 is advised (x RatingFactorMultiplier) more often that disc with rating 2.5
		private const double RatingFactorMultiplier = 2;

		public IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups)
		{
			var rankedGroups = discGroups.Select(dg => new RankedDiscGroup(dg)).ToList();
			return rankedGroups
				.OrderByDescending(CalculateDiscGroupRank)
				.Select(rdg => rdg.BuildDiscGroup());
		}

		public IEnumerable<Disc> SortDiscsWithinGroup(DiscGroup discGroup)
		{
			return discGroup.Discs
				.Where(d => !d.IsDeleted)
				.Select(d => new RankedDisc(d))
				.OrderByDescending(CalculateDiscRankWithinGroup).Select(d => d.Disc);
		}

		private double CalculateDiscGroupRank(RankedDiscGroup discGroup)
		{
			return CalculateGroupFactorForDiscsNumber(discGroup.RankedDiscs.Count(d => !d.Disc.IsDeleted)) *
				   CalculateGroupFactorForAverageDiscRating(discGroup.Rating) *
				   CalculateGroupFactorForPlaybackAge(discGroup.PlaybacksPassed);
		}

		private double CalculateDiscRankWithinGroup(RankedDisc disc)
		{
			return CalculateDiscFactorForRating(disc) * CalculateDiscFactorForPlaybackAge(disc.PlaybacksPassed);
		}

		private static double CalculateGroupFactorForDiscsNumber(int discsNumber)
		{
			return Math.Sqrt(discsNumber);
		}

		private static double CalculateGroupFactorForAverageDiscRating(double averageDiscRating)
		{
			return Math.Pow(RatingFactorMultiplier, averageDiscRating);
		}

		private static double CalculateGroupFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}

		private static double CalculateDiscFactorForRating(RankedDisc disc)
		{
			return Math.Pow(RatingFactorMultiplier, disc.Rating);
		}

		private static double CalculateDiscFactorForPlaybackAge(int playbackAge)
		{
			return playbackAge;
		}
	}
}
