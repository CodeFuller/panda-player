using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker
{
	public interface ILibraryInconsistencyFilter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		bool SkipInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		bool SkipInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		bool SkipInconsistency_SongTitleCorrected(Song song, string correctedSongTitle);
	}
}
