using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscGroupSorter
	{
		IEnumerable<DiscGroup> SortDiscGroups(IEnumerable<DiscGroup> discGroups, PlaybacksInfo playbacksInfo);

		IEnumerable<DiscModel> SortDiscsWithinGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo);
	}
}
