using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;

namespace PandaPlayer.Adviser.Internal
{
	internal class RankBasedAdviseGroupSorter : IAdviseGroupSorter
	{
		private readonly IAdviseRankCalculator adviseRankCalculator;

		public RankBasedAdviseGroupSorter(IAdviseRankCalculator adviseRankCalculator)
		{
			this.adviseRankCalculator = adviseRankCalculator ?? throw new ArgumentNullException(nameof(adviseRankCalculator));
		}

		public IEnumerable<AdviseGroupContent> SortAdviseGroups(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo)
		{
			return adviseGroups
				.OrderByDescending(x => adviseRankCalculator.CalculateAdviseGroupRank(x, playbacksInfo));
		}

		public IEnumerable<AdviseSetContent> SortAdviseSets(IEnumerable<AdviseSetContent> adviseSets, PlaybacksInfo playbacksInfo)
		{
			return adviseSets
				.OrderByDescending(x => adviseRankCalculator.CalculateAdviseSetRank(x, playbacksInfo));
		}
	}
}
