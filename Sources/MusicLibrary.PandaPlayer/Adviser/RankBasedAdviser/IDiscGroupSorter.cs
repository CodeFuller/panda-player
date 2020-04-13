using System.Collections.Generic;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.Adviser.Grouping;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	public interface IDiscGroupSorter
	{
		IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups);

		IEnumerable<Disc> SortDiscsWithinGroup(DiscGroup discGroup);
	}
}
