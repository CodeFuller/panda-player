using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.DiscAdviser
{
	public interface IRankCalculator
	{
		double CalculateArtistRank(LibraryArtist artist);

		double CalculateDiscRank(LibraryDisc disc);
	}
}
