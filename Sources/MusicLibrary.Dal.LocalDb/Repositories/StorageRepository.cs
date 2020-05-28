using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class StorageRepository : IStorageRepository, IContentUriProvider
	{
		private readonly IFileStorageOrganizer storageOrganizer;

		private readonly IFileStorage fileStorage;

		private readonly ISongTagger songTagger;

		private readonly IChecksumCalculator checksumCalculator;

		public StorageRepository(IFileStorageOrganizer storageOrganizer, IFileStorage fileStorage, ISongTagger songTagger, IChecksumCalculator checksumCalculator)
		{
			this.storageOrganizer = storageOrganizer ?? throw new ArgumentNullException(nameof(storageOrganizer));
			this.fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
			this.songTagger = songTagger ?? throw new ArgumentNullException(nameof(songTagger));
			this.checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
		}

		public Task UpdateSongTreeTitle(SongModel oldSong, SongModel newSong, CancellationToken cancellationToken)
		{
			var oldSongPath = storageOrganizer.GetSongFilePath(oldSong);
			var newSongPath = storageOrganizer.GetSongFilePath(newSong);
			fileStorage.MoveFile(oldSongPath, newSongPath);

			newSong.ContentUri = GetSongContentUri(newSong);

			return Task.CompletedTask;
		}

		public Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			var songFileName = fileStorage.CheckoutFile(songPath);

			var tagData = new SongTagData(song);
			songTagger.SetTagData(songFileName, tagData);

			song.Size = CalculateFileSize(songFileName);
			song.Checksum = CalculateFileChecksum(songFileName);

			fileStorage.CommitFile(songFileName);

			return Task.CompletedTask;
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			fileStorage.DeleteFile(songPath);

			// TODO: Delete disc directory if it becomes empty.
			return Task.CompletedTask;
		}

		public Task AddDiscImage(DiscImageModel image, Stream imageContent, CancellationToken cancellationToken)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(image);
			fileStorage.SaveFile(imagePath, imageContent);

			var fullPath = fileStorage.GetFullPath(imagePath);
			image.Size = CalculateFileSize(fullPath);
			image.Checksum = CalculateFileChecksum(fullPath);

			return Task.CompletedTask;
		}

		public Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(image);
			fileStorage.DeleteFile(imagePath);

			// TODO: Delete disc directory if it becomes empty.
			return Task.CompletedTask;
		}

		public Uri GetSongContentUri(SongModel song)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			return GetUriForPath(songPath);
		}

		public Uri GetDiscImageUri(DiscImageModel discImage)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(discImage);
			return GetUriForPath(imagePath);
		}

		private Uri GetUriForPath(FilePath songPath)
		{
			var fullPath = fileStorage.GetFullPath(songPath);
			return new Uri(fullPath);
		}

		private static long CalculateFileSize(string filePath)
		{
			return new FileInfo(filePath).Length;
		}

		private uint CalculateFileChecksum(string filePath)
		{
			return checksumCalculator.CalculateChecksum(filePath);
		}
	}
}
