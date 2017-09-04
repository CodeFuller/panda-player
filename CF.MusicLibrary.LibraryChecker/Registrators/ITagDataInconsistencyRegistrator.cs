using System.Collections.Generic;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ITagDataInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_BadTagData(string inconsistencyMessage);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_BadTagData(Song song, IEnumerable<SongTagType> tagTypes);
	}
}
