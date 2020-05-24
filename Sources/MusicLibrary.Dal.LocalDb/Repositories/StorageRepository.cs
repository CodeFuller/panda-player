using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class StorageRepository : IStorageRepository
	{
		private readonly IFileStorage fileStorage;

		private readonly ISongTagger songTagger;

		private readonly IChecksumCalculator checksumCalculator;

		public StorageRepository(IFileStorage fileStorage, ISongTagger songTagger, IChecksumCalculator checksumCalculator)
		{
			this.fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
			this.songTagger = songTagger ?? throw new ArgumentNullException(nameof(songTagger));
			this.checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
		}

		public Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			var songFileName = fileStorage.CheckoutFile(song.ContentUri);

			var tagData = new SongTagData(song);
			songTagger.SetTagData(songFileName, tagData);

			song.Checksum = checksumCalculator.CalculateChecksum(songFileName);

			fileStorage.CommitFile(songFileName);

			return Task.CompletedTask;
		}
	}
}
