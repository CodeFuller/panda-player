using System.IO;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFileStorage
	{
		void SaveFile(FilePath filePath, Stream content);

		void MoveFile(FilePath source, FilePath destination);

		string CheckoutFile(FilePath filePath);

		void CommitFile(string fileName);

		void DeleteFile(FilePath filePath);

		string GetFullPath(FilePath filePath);

		void MoveFolder(FilePath source, FilePath destination);

		bool FolderIsEmpty(FilePath folderPath);

		void DeleteFolder(FilePath folderPath);
	}
}
