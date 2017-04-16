using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.DiscAdviser
{
	public interface IDiscAdviser
	{
		Collection<LibraryDisc> AdviseNextDiscs(ArtistLibrary library, int discsNumber);
	}
}
