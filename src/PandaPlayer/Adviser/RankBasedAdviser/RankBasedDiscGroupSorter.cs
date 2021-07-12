using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankBasedDiscGroupSorter : IDiscGroupSorter
	{
		private readonly IAdviseRankCalculator adviseRankCalculator;

		public RankBasedDiscGroupSorter(IAdviseRankCalculator adviseRankCalculator)
		{
			this.adviseRankCalculator = adviseRankCalculator ?? throw new ArgumentNullException(nameof(adviseRankCalculator));
		}

		public IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups, PlaybacksInfo playbacksInfo)
		{
			return discGroups
				.Select(dg => new RankedDiscGroup(dg, playbacksInfo))
				.OrderByDescending(rdg => adviseRankCalculator.CalculateDiscGroupRank(rdg))
				.Select(rdg => rdg.DiscGroup);
		}

		public IEnumerable<DiscModel> SortDiscsWithinGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo)
		{
			return discGroup.Discs
				.Where(d => !d.IsDeleted)
				.OrderByDescending(d => adviseRankCalculator.CalculateDiscRank(d, playbacksInfo));
		}
	}
}
