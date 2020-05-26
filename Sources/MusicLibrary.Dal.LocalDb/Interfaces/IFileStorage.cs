using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFileStorage
	{
		void MoveFile(FilePath source, FilePath destination);

		string CheckoutFile(FilePath filePath);

		void CommitFile(string fileName);

		void DeleteFile(FilePath filePath);

		string GetFullPath(FilePath songPath);
	}
}
