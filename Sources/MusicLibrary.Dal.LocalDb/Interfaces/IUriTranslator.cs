using System;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IUriTranslator
	{
		Uri GetExternalUri(Uri internalUri);

		Uri GetInternalUri(Uri externalUri);

		Uri ReplaceSegmentInExternalUri(Uri externalUri, string newValue, int segmentIndex);

		// TODO: Remove after adding folder entity
		Uri AppendSegmentToInternalUri(Uri internalUri, string newSegment);

		// TODO: Remove after adding folder entity
		Uri RemoveLastSegmentFromInternalUri(Uri internalUri);
	}
}
