using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public interface IArtistLibraryBuilder
	{
		ArtistLibrary Build(DiscLibrary discLibrary);
	}
}
