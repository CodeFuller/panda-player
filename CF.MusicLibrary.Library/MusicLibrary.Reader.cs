using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.Library
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibrary
	{
		private readonly IMusicLibraryRepository libraryRepository;

		private readonly IMusicLibraryStorage libraryStorage;

		private readonly ILibraryStructurer libraryStructurer;

		private readonly IChecksumCalculator checksumCalculator;

		public RepositoryAndStorageMusicLibrary(IMusicLibraryRepository libraryRepository, IMusicLibraryStorage libraryStorage,
			ILibraryStructurer libraryStructurer, IChecksumCalculator checksumCalculator)
		{
			if (libraryRepository == null)
			{
				throw new ArgumentNullException(nameof(libraryRepository));
			}
			if (libraryStorage == null)
			{
				throw new ArgumentNullException(nameof(libraryStorage));
			}
			if (libraryStructurer == null)
			{
				throw new ArgumentNullException(nameof(libraryStructurer));
			}
			if (checksumCalculator == null)
			{
				throw new ArgumentNullException(nameof(checksumCalculator));
			}

			this.libraryRepository = libraryRepository;
			this.libraryStorage = libraryStorage;
			this.libraryStructurer = libraryStructurer;
			this.checksumCalculator = checksumCalculator;
		}

		public async Task<IEnumerable<Disc>> LoadDiscs()
		{
			return await libraryRepository.GetDiscs();
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

		public async Task CheckStorage(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			await libraryStorage.CheckDataConsistency(library, registrator, fixFoundIssues);
		}

		public async Task CheckStorageChecksums(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			foreach (var song in library.Songs)
			{
				string songFileName = await libraryStorage.GetSongFile(song);
				var currChecksum = checksumCalculator.CalculateChecksumForFile(songFileName);
				if (currChecksum != song.Checksum)
				{
					registrator.RegisterInconsistency_LibraryData($"Checksum mismatch: 0x{currChecksum:X8} != 0x{song.Checksum:X8} for song {song.Uri}");
					if (fixFoundIssues)
					{
						song.Checksum = currChecksum;
						await libraryRepository.UpdateSong(song);
						Logger.WriteInfo($"Checksum has been updated for song '{song.Uri}'");
					}
				}
			}
		}
	}
}
