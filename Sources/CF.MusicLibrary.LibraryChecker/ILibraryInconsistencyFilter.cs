using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker
{
	public interface ILibraryInconsistencyFilter
	{
		bool ShouldSkipDifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);

		bool ShouldSkipArtistNameCorrection(string originalArtistName, string correctedArtistName);

		bool ShouldSkipSongTitleCorrection(Song song, string correctedSongTitle);
	}
}
