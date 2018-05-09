using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Universal;

namespace CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	public class RankBasedDiscGroupSorter : IDiscGroupSorter
	{
		private readonly IAdviseFactorsProvider adviseFactorsProvider;

		public RankBasedDiscGroupSorter(IAdviseFactorsProvider adviseFactorsProvider)
		{
			if (adviseFactorsProvider == null)
			{
				throw new ArgumentNullException(nameof(adviseFactorsProvider));
			}

			this.adviseFactorsProvider = adviseFactorsProvider;
		}

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
			return discGroup.PlaybacksPassed == Int32.MaxValue ? Double.MaxValue :
					adviseFactorsProvider.GetFactorForGroupDiscsNumber(discGroup.RankedDiscs.Count(d => !d.Disc.IsDeleted)) *
					adviseFactorsProvider.GetFactorForAverageRating(discGroup.Rating) *
					adviseFactorsProvider.GetFactorForPlaybackAge(discGroup.PlaybacksPassed);
		}

		private double CalculateDiscRankWithinGroup(RankedDisc disc)
		{
			return adviseFactorsProvider.GetFactorForAverageRating(disc.Rating) * adviseFactorsProvider.GetFactorForPlaybackAge(disc.PlaybacksPassed);
		}
	}
}
