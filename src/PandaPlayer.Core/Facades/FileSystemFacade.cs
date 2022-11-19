using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PandaPlayer.Core.Facades
{
	public class FileSystemFacade : IFileSystemFacade
	{
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public void MoveDirectory(string sourceDirName, string destDirName)
		{
			Directory.Move(sourceDirName, destDirName);
		}

		public void DeleteDirectory(string path)
		{
			Directory.Delete(path);
		}

		public void DeleteDirectory(string path, bool recursive)
		{
			Directory.Delete(path, recursive);
		}

		public IEnumerable<string> EnumerateDirectories(string path)
		{
			return Directory.EnumerateDirectories(path);
		}

		public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateDirectories(path, searchPattern, searchOption);
		}

		public IEnumerable<string> EnumerateFiles(string path)
		{
			return Directory.EnumerateFiles(path);
		}

		public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFiles(path, searchPattern, searchOption);
		}

		public bool DirectoryIsEmpty(string path)
		{
			return !Directory.EnumerateFileSystemEntries(path).Any();
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void MoveFile(string sourceFileName, string destFileName)
		{
			File.Move(sourceFileName, destFileName);
		}

		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}

		public bool GetReadOnlyAttribute(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			return fileInfo.IsReadOnly;
		}

		public void ClearReadOnlyAttribute(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			fileInfo.IsReadOnly = false;
		}

		public void SetReadOnlyAttribute(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			fileInfo.IsReadOnly = true;
		}

		public long GetFileSize(string fileName)
		{
			return new FileInfo(fileName).Length;
		}

		public byte[] ReadAllBytes(string path)
		{
			return File.ReadAllBytes(path);
		}

		public void WriteAllBytes(string path, byte[] bytes)
		{
			File.WriteAllBytes(path, bytes);
		}

		public string GetTempFileName()
		{
			return Path.GetTempFileName();
		}
	}
}
