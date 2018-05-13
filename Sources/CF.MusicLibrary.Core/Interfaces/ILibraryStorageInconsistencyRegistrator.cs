using System;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface ILibraryStorageInconsistencyRegistrator
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_MissingStorageData(Uri itemUri);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_UnexpectedStorageData(string itemPath, string itemType);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterInconsistency_ErrorInStorageData(string errorMessage);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name is unreadable without underscore")]
		void RegisterFix_ErrorInStorageData(string fixMessage);
	}
}
