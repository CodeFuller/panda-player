using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Universal.Interfaces
{
	public interface IDiscGroupper
	{
		IEnumerable<DiscGroup> GroupLibraryDiscs(DiscLibrary library);
	}
}
