using System;
using System.Collections.Generic;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IFileStorage
	{
		void StoreFile(string sourceFileName, Uri fileUri);

		string GetFile(Uri fileUri);

		string GetFileForWriting(Uri fileUri);

		void UpdateFileContent(string sourceFileName, Uri fileUri);

		void MoveFile(Uri currFileUri, Uri newFileUri);

		void MoveDirectory(Uri currDirectoryUri, Uri newDirectoryUri);

		void DeleteFile(Uri fileUri);

		void CheckDataConsistency(IEnumerable<Uri> expectedFileUris, IEnumerable<Uri> ignoreList, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues);
	}
}
