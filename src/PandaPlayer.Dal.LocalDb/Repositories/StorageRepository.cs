using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Tagging;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal partial class StorageRepository : IStorageRepository, IContentUriProvider
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

		public Task CreateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			var discFolderPath = storageOrganizer.GetDiscFolderPath(disc);
			fileStorage.CreateFolder(discFolderPath);

			return Task.CompletedTask;
		}

		public Task UpdateDiscTreeTitle(DiscModel oldDisc, DiscModel newDisc, CancellationToken cancellationToken)
		{
			var oldDiscPath = storageOrganizer.GetDiscFolderPath(oldDisc);
			var newDiscPath = storageOrganizer.GetDiscFolderPath(newDisc);
			fileStorage.MoveFolder(oldDiscPath, newDiscPath);

			UpdateContentUris(newDisc);

			return Task.CompletedTask;
		}

		public Task AddSong(SongModel song, Stream songContent, CancellationToken cancellationToken)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			fileStorage.SaveFile(songPath, songContent);

			UpdateSongTags(song);

			SetSongContentUri(song);

			return Task.CompletedTask;
		}

		public Task UpdateSongTreeTitle(SongModel oldSong, SongModel newSong, CancellationToken cancellationToken)
		{
			var oldSongPath = storageOrganizer.GetSongFilePath(oldSong);
			var newSongPath = storageOrganizer.GetSongFilePath(newSong);
			fileStorage.MoveFile(oldSongPath, newSongPath);

			SetSongContentUri(newSong);

			return Task.CompletedTask;
		}

		public Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			UpdateSongTags(song);

			return Task.CompletedTask;
		}

		private void UpdateSongTags(SongModel song)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			var songFileName = fileStorage.CheckoutFile(songPath);

			var tagData = new SongTagData(song);
			songTagger.SetTagData(songFileName, tagData);

			song.Size = CalculateFileSize(songFileName);
			song.Checksum = CalculateFileChecksum(songFileName);

			fileStorage.CommitFile(songFileName);
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			fileStorage.DeleteFile(songPath);

			var folderPath = GetFolderPath(songPath);
			DeleteFolderIfEmpty(folderPath);

			return Task.CompletedTask;
		}

		public Task AddDiscImage(DiscImageModel image, Stream imageContent, CancellationToken cancellationToken)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(image);
			fileStorage.SaveFile(imagePath, imageContent);

			var fullPath = fileStorage.GetFullPath(imagePath);
			image.Size = CalculateFileSize(fullPath);
			image.Checksum = CalculateFileChecksum(fullPath);

			SetDiscImageUri(image);

			return Task.CompletedTask;
		}

		public Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(image);
			fileStorage.DeleteFile(imagePath);

			var folderPath = GetFolderPath(imagePath);
			DeleteFolderIfEmpty(folderPath);

			return Task.CompletedTask;
		}

		public Task CreateFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			var folderPath = storageOrganizer.GetFolderPath(folder);
			fileStorage.CreateFolder(folderPath);

			return Task.CompletedTask;
		}

		public Task RenameFolder(FolderModel oldFolder, FolderModel newFolder, CancellationToken cancellationToken)
		{
			var oldFolderPath = storageOrganizer.GetFolderPath(oldFolder);
			var newFolderPath = storageOrganizer.GetFolderPath(newFolder);
			fileStorage.MoveFolder(oldFolderPath, newFolderPath);

			UpdateContentUris(newFolder);

			return Task.CompletedTask;
		}

		private void UpdateContentUris(FolderModel folder)
		{
			foreach (var subfolder in folder.Subfolders)
			{
				UpdateContentUris(subfolder);
			}

			foreach (var disc in folder.Discs)
			{
				UpdateContentUris(disc);
			}
		}

		private void UpdateContentUris(DiscModel disc)
		{
			foreach (var song in disc.ActiveSongs)
			{
				SetSongContentUri(song);
			}

			foreach (var image in disc.Images)
			{
				SetDiscImageUri(image);
			}
		}

		public Task DeleteFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			var folderPath = storageOrganizer.GetFolderPath(folder);
			fileStorage.DeleteFolder(folderPath);

			return Task.CompletedTask;
		}

		public void SetSongContentUri(SongModel song)
		{
			if (song.IsDeleted)
			{
				throw new InvalidOperationException("Can not build content URL for deleted song");
			}

			var songPath = storageOrganizer.GetSongFilePath(song);
			song.ContentUri = GetUriForPath(songPath);
		}

		public void SetDiscImageUri(DiscImageModel discImage)
		{
			var imagePath = storageOrganizer.GetDiscImagePath(discImage);
			discImage.ContentUri = GetUriForPath(imagePath);
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

		private static FilePath GetFolderPath(FilePath filePath)
		{
			var parts = filePath.ToList();
			return new FilePath(parts.Take(parts.Count - 1));
		}

		private void DeleteFolderIfEmpty(FilePath folderPath)
		{
			if (!fileStorage.FolderIsEmpty(folderPath))
			{
				return;
			}

			fileStorage.DeleteFolder(folderPath);
		}
	}
}
