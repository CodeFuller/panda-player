using System;
using System.Collections.Generic;
using System.IO;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Diagnostic.Inconsistencies;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	internal interface IFileStorage
	{
		bool FileExists(FilePath filePath);

		void SaveFile(FilePath filePath, Stream content);

		void MoveFile(FilePath source, FilePath destination);

		string CheckoutFile(FilePath filePath);

		void CommitFile(string fileName);

		void DeleteFile(FilePath filePath);

		string GetFullPath(FilePath filePath);

		bool FolderExists(FilePath folderPath);

		bool FolderIsEmpty(FilePath folderPath);

		void CreateFolder(FilePath folderPath);

		void MoveFolder(FilePath source, FilePath destination);

		void DeleteFolder(FilePath folderPath);

		IEnumerable<string> EnumerateFolders();

		IEnumerable<string> EnumerateFiles();

		void CheckFile(FilePath songPath, Action<LibraryInconsistency> inconsistenciesHandler);
	}
}
