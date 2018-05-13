using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker
{
	public interface ILibraryInconsistencyFilter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		bool SkipInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		bool SkipInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		bool SkipInconsistency_SongTitleCorrected(Song song, string correctedSongTitle);
	}
}
