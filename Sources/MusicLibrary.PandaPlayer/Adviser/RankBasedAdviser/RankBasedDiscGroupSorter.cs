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
		private readonly IAdviseFactorsProvider adviseFactorsProvider;

		public RankBasedDiscGroupSorter(IAdviseFactorsProvider adviseFactorsProvider)
		{
			this.adviseFactorsProvider = adviseFactorsProvider ?? throw new ArgumentNullException(nameof(adviseFactorsProvider));
		}

		public IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups, PlaybacksInfo playbacksInfo)
		{
			var rankedGroups = discGroups.Select(dg => new RankedDiscGroup(dg, playbacksInfo)).ToList();
			return rankedGroups
				.OrderByDescending(CalculateDiscGroupRank)
				.Select(rdg => rdg.BuildDiscGroup());
		}

		public IEnumerable<DiscModel> SortDiscsWithinGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo)
		{
			return discGroup.Discs
				.Where(d => !d.IsDeleted)
				.Select(d => new RankedDisc(d, playbacksInfo.GetPlaybacksPassed(d)))
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
