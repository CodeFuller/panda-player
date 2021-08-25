using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Grouping;

namespace PandaPlayer.Adviser.Internal
{
	internal class RankedAdviseGroup
	{
		public AdviseGroupContent AdviseGroup { get; }

		public IReadOnlyCollection<AdviseSetContent> AdviseSets => AdviseGroup.AdviseSets;

		public int PlaybacksPassed { get; }

		public double Rating => AdviseSets
			.Where(x => !x.IsDeleted)
			.Select(x => x.Rating)
			.Average();

		public RankedAdviseGroup(AdviseGroupContent adviseGroup, PlaybacksInfo playbacksInfo)
		{
			AdviseGroup = adviseGroup ?? throw new ArgumentNullException(nameof(adviseGroup));

			PlaybacksPassed = AdviseSets.Select(playbacksInfo.GetPlaybacksPassed).Min();
		}
	}
}
