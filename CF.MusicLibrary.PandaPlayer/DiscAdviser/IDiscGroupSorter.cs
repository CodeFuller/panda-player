using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Universal;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public interface IDiscGroupSorter
	{
		IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups);

		IEnumerable<Disc> SortDiscsWithinGroup(DiscGroup discGroup);
	}
}
