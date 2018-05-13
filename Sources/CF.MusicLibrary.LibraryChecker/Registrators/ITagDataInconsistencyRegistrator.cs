using System.Collections.Generic;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ITagDataInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_BadTagData(string inconsistencyMessage);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_BadTagData(Song song, IEnumerable<SongTagType> tagTypes);
	}
}
