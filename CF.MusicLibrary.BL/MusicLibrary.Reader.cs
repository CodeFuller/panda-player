using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibrary
	{
		private readonly IMusicLibraryRepository libraryRepository;

		private readonly IMusicLibraryStorage libraryStorage;

		public RepositoryAndStorageMusicLibrary(IMusicLibraryRepository libraryRepository, IMusicLibraryStorage libraryStorage)
		{
			if (libraryRepository == null)
			{
				throw new ArgumentNullException(nameof(libraryRepository));
			}
			if (libraryStorage == null)
			{
				throw new ArgumentNullException(nameof(libraryStorage));
			}

			this.libraryRepository = libraryRepository;
			this.libraryStorage = libraryStorage;
		}

		public async Task<DiscLibrary> Load()
		{
			return new DiscLibrary(await GetDiscsAsync());
		}

		public async Task<IEnumerable<Disc>> GetDiscsAsync()
		{
			return await libraryRepository.GetDiscsAsync();
		}

		public async Task<SongTagData> GetSongTagData(Song song)
		{
			return await libraryStorage.GetSongTagData(song);
		}

		public async Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song)
		{
			return await libraryStorage.GetSongTagTypes(song);
		}

		public async Task<FileInfo> GetSongFile(Song song)
		{
			return await libraryStorage.GetSongFile(song);
		}

		public async Task<bool> CheckSongContent(Song song)
		{
			return await libraryStorage.CheckSongContent(song);
		}
	}
}
