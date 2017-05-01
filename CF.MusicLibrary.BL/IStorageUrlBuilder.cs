using System;

namespace CF.MusicLibrary.BL
{
	public interface IStorageUrlBuilder
	{
		Uri BuildArtistStorageUrl(params string[] segments);

		Uri BuildAlbumStorageUrl(Uri artistUri, string albumName);

		Uri BuildSongStorageUrl(Uri albumUri, string songName);

		Uri ReplaceArtistName(Uri artistStorageUri, string newArtistName);

		Uri ReplaceAlbumName(Uri albumStorageUri, string newAlbumName);

		Uri MapWorkshopStoragePath(string pathWithinStorage);
	}
}
