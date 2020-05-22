using System;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IDataStorage
	{
		Uri TranslateInternalUri(Uri internalUri);
	}
}
