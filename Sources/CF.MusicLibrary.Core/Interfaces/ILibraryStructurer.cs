using System;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface ILibraryStructurer
	{
		Uri BuildSongUri(Uri discUri, string songFileName);

		Uri ReplaceDiscPartInUri(Uri discUri, string discPart);

		Uri ReplaceDiscPartInSongUri(Uri newDiscUri, Uri songUri);

		Uri ReplaceDiscPartInImageUri(Uri newDiscUri, Uri imageUri);

		string GetDiscFolderName(Uri discUri);

		string GetFileNameFromUri(Uri songUri);

		Uri GetDiscCoverImageUri(Uri discUri, ImageInfo imageInfo);
	}
}
