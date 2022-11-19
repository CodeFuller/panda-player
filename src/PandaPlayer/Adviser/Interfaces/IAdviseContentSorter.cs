using System.Collections.Generic;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IAdviseContentSorter
	{
		IEnumerable<AdviseGroupContent> SortAdviseGroups(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo);

		IEnumerable<AdviseSetContent> SortAdviseSets(IEnumerable<AdviseSetContent> adviseSets, PlaybacksInfo playbacksInfo);
	}
}
