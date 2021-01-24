using System.Collections.Generic;
using System.IO;

namespace MusicLibrary.Core.Facades
{
	public interface IFileSystemFacade
	{
		bool DirectoryExists(string path);

		void CreateDirectory(string path);

		void MoveDirectory(string sourceDirName, string destDirName);

		void DeleteDirectory(string path);

		void DeleteDirectory(string path, bool recursive);

		IEnumerable<string> EnumerateDirectories(string path);

		IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

		IEnumerable<string> EnumerateFiles(string path);

		IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);

		bool DirectoryIsEmpty(string path);

		bool FileExists(string path);

		void MoveFile(string sourceFileName, string destFileName);

		void DeleteFile(string fileName);

		bool GetReadOnlyAttribute(string fileName);

		void ClearReadOnlyAttribute(string fileName);

		void SetReadOnlyAttribute(string fileName);

		long GetFileSize(string fileName);

		byte[] ReadAllBytes(string path);

		void WriteAllBytes(string path, byte[] bytes);

		string GetTempFileName();
	}
}
