using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_SuspiciousAlbumTitle(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_DiscWithoutSongs(Disc disc);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_BadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);
	}
}
