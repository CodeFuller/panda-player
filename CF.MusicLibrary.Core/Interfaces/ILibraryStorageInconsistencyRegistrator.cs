using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface ILibraryStorageInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_MissingSongData(Song song);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		void RegisterInconsistency_LibraryData(string inconsistencyMessage);
	}
}
