using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	/// <summary>
	/// Implementation of IMusicLibrary with separated storing of music data (IMusicCatalog) and song content (IMusicStorage).
	/// </summary>
	public class CatalogBasedMusicLibrary : IMusicLibrary
	{
		private readonly IMusicCatalog musicCatalog;
		private readonly IMusicStorage musicStorage;
		private readonly IArtistLibraryBuilder artistLibraryBuilder;
		private readonly IStorageUrlBuilder storageUrlBuilder;

		public ArtistLibrary ArtistLibrary { get; internal set; }

		public CatalogBasedMusicLibrary(IMusicCatalog musicCatalog, IMusicStorage musicStorage, IArtistLibraryBuilder artistLibraryBuilder, IStorageUrlBuilder storageUrlBuilder)
		{
			if (musicCatalog == null)
			{
				throw new ArgumentNullException(nameof(musicCatalog));
			}
			if (musicStorage == null)
			{
				throw new ArgumentNullException(nameof(musicStorage));
			}
			if (artistLibraryBuilder == null)
			{
				throw new ArgumentNullException(nameof(artistLibraryBuilder));
			}
			if (storageUrlBuilder == null)
			{
				throw new ArgumentNullException(nameof(storageUrlBuilder));
			}

			this.musicCatalog = musicCatalog;
			this.musicStorage = musicStorage;
			this.artistLibraryBuilder = artistLibraryBuilder;
			this.storageUrlBuilder = storageUrlBuilder;
		}

		public async Task LoadAsync()
		{
			DiscLibrary discLibrary = await musicCatalog.GetDiscsAsync();
			ArtistLibrary = artistLibraryBuilder.Build(discLibrary);
		}

		public async Task AddSong(Song song, string sourceFilepath)
		{
			await musicStorage.AddSongAsync(sourceFilepath, song.Uri);
		}

		public async Task SetAlbumCoverImage(Uri albumUri, string coverImagePath)
		{
			await musicStorage.SetAlbumCoverImage(albumUri, coverImagePath);
		}

		public async Task<IEnumerable<Genre>> GetGenresAsync()
		{
			return (ArtistLibrary?.Songs.Select(s => s.Genre) ?? await musicCatalog.GetAllGenresAsync()).
				Distinct().
				OrderBy(g => g.Name);
		}

		public IEnumerable<Uri> GetAvailableArtistStorageUris(string artist)
		{
			return ArtistLibrary.Artists.
				Select(a => a.StorageUri).
				Where(u => u!= null).
				Select(u => storageUrlBuilder.ReplaceArtistName(u, artist)).
				Distinct();
		}

		public Uri GetArtistStorageUri(string artist)
		{
			return ArtistLibrary.GetArtist(artist)?.StorageUri;
		}
	}
}
