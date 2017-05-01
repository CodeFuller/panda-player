using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibrary
	{
		ArtistLibrary ArtistLibrary { get; }

		Task LoadAsync();

		Task AddSong(Song song, string sourceFilepath);

		Task SetAlbumCoverImage(Uri albumUri, string coverImagePath);

		Task<IEnumerable<Genre>> GetGenresAsync();

		IEnumerable<Uri> GetAvailableArtistStorageUris(string artist);

		Uri GetArtistStorageUri(string artist);
	}
}
