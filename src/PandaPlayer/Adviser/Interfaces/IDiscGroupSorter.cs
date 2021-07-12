using System.Collections.Generic;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscGroupSorter
	{
		IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups, PlaybacksInfo playbacksInfo);

		IEnumerable<DiscModel> SortDiscsWithinGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo);
	}
}
