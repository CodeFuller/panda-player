using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface ILibraryStructurer
	{
		IEnumerable<Uri> GetAllPossibleArtistStorageUris(Artist artist);

		Uri GetArtistStorageUri(DiscLibrary library, Artist artist);

		Uri BuildArtistDiscUri(Uri artistUri, string discNamePart);

		Uri BuildSongUri(Uri discUri, string songFileName);

		Uri BuildUriForWorkshopStoragePath(string pathWithinStorage);

		Uri ReplaceDiscPartInUri(Uri discUri, string discPart);
	}
}
