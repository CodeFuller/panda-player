using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
{
	public interface IDiscGroupper
	{
		IEnumerable<DiscGroup> GroupLibraryDiscs(DiscLibrary library);
	}
}
