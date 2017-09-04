using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.DiscAdviser
{
	public interface IDiscAdviser
	{
		Collection<Disc> AdviseDiscs(DiscLibrary library);
	}
}
