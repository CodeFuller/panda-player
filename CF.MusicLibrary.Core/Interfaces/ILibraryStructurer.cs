using System;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface ILibraryStructurer
	{
		Uri BuildSongUri(Uri discUri, string songFileName);

		Uri ReplaceDiscPartInUri(Uri discUri, string discPart);

		Uri ReplaceDiscPartInSongUri(Uri newDiscUri, Uri songUri);

		string GetDiscFolderName(Uri discUri);

		string GetSongFileName(Uri songUri);
	}
}
