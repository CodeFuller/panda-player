using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
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
