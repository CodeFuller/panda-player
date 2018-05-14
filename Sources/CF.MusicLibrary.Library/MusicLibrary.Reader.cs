using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.Library
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibrary
	{
		private readonly IMusicLibraryRepository libraryRepository;
		private readonly IMusicLibraryStorage libraryStorage;
		private readonly ISongTagger songTagger;
		private readonly ILibraryStructurer libraryStructurer;
		private readonly IChecksumCalculator checksumCalculator;
		private readonly ILogger<RepositoryAndStorageMusicLibrary> logger;

		/// <summary>
		/// Gets or sets IClock dependency.
		/// </summary>
		public IClock DateTimeFacade { get; set; } = new SystemClock();

		public RepositoryAndStorageMusicLibrary(IMusicLibraryRepository libraryRepository, IMusicLibraryStorage libraryStorage, ISongTagger songTagger,
			ILibraryStructurer libraryStructurer, IChecksumCalculator checksumCalculator, ILogger<RepositoryAndStorageMusicLibrary> logger)
		{
			this.libraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
			this.libraryStorage = libraryStorage ?? throw new ArgumentNullException(nameof(libraryStorage));
			this.songTagger = songTagger ?? throw new ArgumentNullException(nameof(songTagger));
			this.libraryStructurer = libraryStructurer ?? throw new ArgumentNullException(nameof(libraryStructurer));
			this.checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IEnumerable<Disc>> LoadDiscs()
		{
			return await libraryRepository.GetDiscs();
		}

		public async Task<DiscLibrary> LoadLibrary()
		{
			return new DiscLibrary(await LoadDiscs());
		}

		public async Task<string> GetSongFile(Song song)
		{
			return await libraryStorage.GetSongFile(song);
		}

		public async Task<SongTagData> GetSongTagData(Song song)
		{
			var songFileName = await libraryStorage.GetSongFile(song);
			return await Task.Run(() => songTagger.GetTagData(songFileName));
		}

		public async Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song)
		{
			var songFileName = await libraryStorage.GetSongFile(song);
			return await Task.Run(() => songTagger.GetTagTypes(songFileName));
		}

		public async Task<string> GetDiscCoverImage(Disc disc)
		{
			var coverImage = disc.CoverImage;
			return coverImage == null ? null : await libraryStorage.GetDiscImageFile(disc.CoverImage);
		}

		public async Task CheckStorage(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			var expectedItems = new List<Uri>();
			foreach (var disc in library.Discs)
			{
				expectedItems.AddRange(disc.Songs.Select(s => s.Uri));
				expectedItems.AddRange(disc.Images.Select(im => im.Uri));
			}

			await libraryStorage.CheckDataConsistency(expectedItems, registrator, fixFoundIssues);
		}

		public async Task CheckStorageChecksums(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues)
		{
			foreach (var disc in library.Discs)
			{
				foreach (var song in disc.Songs)
				{
					string songFileName = await libraryStorage.GetSongFile(song);
					var currChecksum = checksumCalculator.CalculateChecksumForFile(songFileName);
					if (currChecksum != song.Checksum)
					{
						registrator.RegisterErrorInStorageData($"Checksum mismatch: 0x{currChecksum:X8} != 0x{song.Checksum:X8} for song {song.Uri}");
						if (fixFoundIssues)
						{
							song.Checksum = currChecksum;
							await libraryRepository.UpdateSong(song);
							logger.LogInformation($"Checksum has been updated for song '{song.Uri}'");
						}
					}
				}

				foreach (var image in disc.Images)
				{
					string imageFileName = await libraryStorage.GetDiscImageFile(image);
					var currChecksum = checksumCalculator.CalculateChecksumForFile(imageFileName);
					if (currChecksum != image.Checksum)
					{
						registrator.RegisterErrorInStorageData($"Checksum mismatch: 0x{currChecksum:X8} != 0x{image.Checksum:X8} for image {image.Uri}");
						if (fixFoundIssues)
						{
							image.Checksum = currChecksum;
							await libraryRepository.UpdateDiscImage(image);
							logger.LogInformation($"Checksum has been updated for image '{image.Uri}'");
						}
					}
				}
			}
		}
	}
}
