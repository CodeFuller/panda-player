using System;
using System.Collections.Generic;
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

		private readonly ILibraryStructurer libraryStructurer;

		public RepositoryAndStorageMusicLibrary(IMusicLibraryRepository libraryRepository, IMusicLibraryStorage libraryStorage, ILibraryStructurer libraryStructurer)
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
			this.libraryStructurer = libraryStructurer;
		}

		public async Task<IEnumerable<Disc>> LoadDiscs()
		{
			return await libraryRepository.GetDiscsAsync();
		}

		public async Task<DiscLibrary> LoadLibrary()
		{
			return new DiscLibrary(await LoadDiscs());
		}

		public async Task<SongTagData> GetSongTagData(Song song)
		{
			return await libraryStorage.GetSongTagData(song);
		}

		public async Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song)
		{
			return await libraryStorage.GetSongTagTypes(song);
		}

		public async Task<string> GetSongFile(Song song)
		{
			return await libraryStorage.GetSongFile(song);
		}

		public async Task<string> GetDiscCoverImage(Disc disc)
		{
			return await libraryStorage.GetDiscCoverImage(disc);
		}
		
		public async Task CheckStorage(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator)
		{
			await libraryStorage.CheckDataConsistency(library, registrator);
		}
	}
}
