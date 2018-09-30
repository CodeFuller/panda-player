using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.Grouping
{
	public interface IDiscGroupper
	{
		IEnumerable<DiscGroup> GroupLibraryDiscs(DiscLibrary library);
	}
}
