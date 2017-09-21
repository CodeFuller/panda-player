using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
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

		public async Task<DiscLibrary> Load()
		{
			return await Load(false);
		}

		public async Task<DiscLibrary> Load(bool includeDeleted)
		{
			return new DiscLibrary(await GetDiscsAsync(includeDeleted));
		}

		public async Task<IEnumerable<Disc>> GetDiscsAsync()
		{
			return await GetDiscsAsync(false);
		}

		public async Task<IEnumerable<Disc>> GetDiscsAsync(bool includeDeleted)
		{
			var discs = await libraryRepository.GetDiscsAsync();

			if (includeDeleted)
			{
				return discs;
			}

			//	Removing deleted songs from discs where some songs are still not deleted
			var resultDiscs = new List<Disc>();
			foreach (var disc in discs.Where(d => !d.IsDeleted))
			{
				disc.SongsUnordered = disc.SongsUnordered.Where(s => !s.IsDeleted).ToCollection();
				resultDiscs.Add(disc);
			}
			return resultDiscs;
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
