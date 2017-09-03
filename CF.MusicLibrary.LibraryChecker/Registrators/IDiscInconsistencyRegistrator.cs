using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_SuspiciousAlbumTitle(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DiscWithoutSongs(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_BadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_BadSongContent(Song song);
	}
}
