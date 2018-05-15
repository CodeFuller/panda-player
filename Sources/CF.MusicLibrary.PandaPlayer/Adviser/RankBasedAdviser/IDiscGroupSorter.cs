using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser.Grouping;

namespace CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	public interface IDiscGroupSorter
	{
		IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups);

		IEnumerable<Disc> SortDiscsWithinGroup(DiscGroup discGroup);
	}
}
