using System;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFileStorage
	{
		string CheckoutFile(Uri fileUri);

		void CommitFile(string fileName);
	}
}
