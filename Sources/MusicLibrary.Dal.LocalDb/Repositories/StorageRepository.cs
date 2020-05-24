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

		private readonly IUriTranslator uriTranslator;

		private readonly ISongTagger songTagger;

		private readonly IChecksumCalculator checksumCalculator;

		public StorageRepository(IFileStorage fileStorage, IUriTranslator uriTranslator, ISongTagger songTagger, IChecksumCalculator checksumCalculator)
		{
			this.fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
			this.uriTranslator = uriTranslator ?? throw new ArgumentNullException(nameof(uriTranslator));
			this.songTagger = songTagger ?? throw new ArgumentNullException(nameof(songTagger));
			this.checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
		}

		public Task UpdateSongTreeTitle(SongModel newSong, Uri currentSongContentUri, CancellationToken cancellationToken)
		{
			var newContentUri = uriTranslator.ReplaceSegmentInExternalUri(currentSongContentUri, newSong.TreeTitle, -1);

			newSong.ContentUri = newContentUri;
			fileStorage.MoveFile(currentSongContentUri, newContentUri);

			return Task.CompletedTask;
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

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			fileStorage.DeleteFile(song.ContentUri);
			return Task.CompletedTask;
		}
	}
}
