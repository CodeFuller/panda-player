using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public interface IDiscAdviser
	{
		Collection<Disc> AdviseDiscs(DiscLibrary library);
	}
}
