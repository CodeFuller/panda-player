using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscInconsistencyRegistrator
	{
		void RegisterSuspiciousAlbumTitle(Disc disc);

		void RegisterDiscWithoutSongs(Disc disc);

		void RegisterBadTrackNumbersForDisc(Disc disc, IEnumerable<short?> trackNumbers);

		void RegisterDifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres);
	}
}
