using System;

namespace MusicLibrary.Core.Interfaces
{
	public interface ILibraryStorageInconsistencyRegistrator
	{
		void RegisterMissingStorageData(Uri itemUri);

		void RegisterUnexpectedStorageData(string itemPath, string itemType);

		void RegisterErrorInStorageData(string errorMessage);

		void RegisterFixOfErrorInStorageData(string fixMessage);
	}
}
