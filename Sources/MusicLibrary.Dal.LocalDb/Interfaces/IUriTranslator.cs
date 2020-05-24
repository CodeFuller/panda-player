using System;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IUriTranslator
	{
		Uri GetExternalUri(Uri internalUri);

		Uri GetInternalUri(Uri externalUri);
	}
}
