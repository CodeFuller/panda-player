using System;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFileStorage
	{
		void MoveFile(Uri currentFileUri, Uri newFileUri);

		string CheckoutFile(Uri fileUri);

		void CommitFile(string fileName);

		void DeleteFile(Uri fileUri);
	}
}
