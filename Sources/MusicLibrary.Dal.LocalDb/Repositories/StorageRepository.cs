using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Inconsistencies;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
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

		public Task UpdateDiscTreeTitle(DiscModel oldDisc, DiscModel newDisc, CancellationToken cancellationToken)
		{
			var oldDiscPath = storageOrganizer.GetDiscFolderPath(oldDisc);
			var newDiscPath = storageOrganizer.GetDiscFolderPath(newDisc);
			fileStorage.MoveFolder(oldDiscPath, newDiscPath);

			foreach (var song in newDisc.ActiveSongs)
			{
				song.ContentUri = GetSongContentUri(song);
			}

			foreach (var image in newDisc.Images)
			{
				image.ContentUri = GetDiscImageUri(image);
			}

			return Task.CompletedTask;
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

			image.ContentUri = GetDiscImageUri(image);

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

		public Task DeleteFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			var folderPath = storageOrganizer.GetFolderPath(folder);
			fileStorage.DeleteFolder(folderPath);

			return Task.CompletedTask;
		}

		public Task CheckStorage(LibraryCheckFlags checkFlags, IEnumerable<ShallowFolderModel> folders, IEnumerable<DiscModel> discs, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			var knownFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			CheckFolders(folders, knownFolders, inconsistenciesHandler);

			var checkContent = checkFlags.HasFlag(LibraryCheckFlags.CheckContentConsistency);
			var knownFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			CheckDiscsData(discs, checkContent, knownFolders, knownFiles, inconsistenciesHandler);

			CheckForUnexpectedFolders(knownFolders, inconsistenciesHandler);

			CheckForUnexpectedFiles(knownFiles, inconsistenciesHandler);

			return Task.CompletedTask;
		}

		private void CheckFolders(IEnumerable<ShallowFolderModel> folders, HashSet<string> visitedFolders, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			foreach (var folder in folders)
			{
				var folderPath = storageOrganizer.GetFolderPath(folder);
				CheckStorageFolder(folderPath, inconsistenciesHandler);

				visitedFolders.Add(fileStorage.GetFullPath(folderPath));
			}
		}

		private void CheckDiscsData(IEnumerable<DiscModel> discs, bool checkContent, HashSet<string> visitedFolders, HashSet<string> visitedFiles, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			foreach (var disc in discs)
			{
				var discFolderPath = storageOrganizer.GetDiscFolderPath(disc);
				CheckStorageFolder(discFolderPath, inconsistenciesHandler);
				visitedFolders.Add(fileStorage.GetFullPath(discFolderPath));

				foreach (var song in disc.ActiveSongs)
				{
					var songPath = storageOrganizer.GetSongFilePath(song);
					CheckStorageFile(songPath, checkContent, song.Size.Value, song.Checksum.Value, inconsistenciesHandler);

					visitedFiles.Add(fileStorage.GetFullPath(songPath));
				}

				foreach (var image in disc.Images)
				{
					var imagePath = storageOrganizer.GetDiscImagePath(image);
					CheckStorageFile(imagePath, checkContent, image.Size, image.Checksum, inconsistenciesHandler);

					visitedFiles.Add(fileStorage.GetFullPath(imagePath));
				}
			}
		}

		private void CheckForUnexpectedFolders(HashSet<string> allowedFolders, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			foreach (var fullFolderPath in fileStorage.EnumerateFolders())
			{
				if (!allowedFolders.Contains(fullFolderPath))
				{
					inconsistenciesHandler(new UnexpectedFolderInconsistency(fullFolderPath));
				}
			}
		}

		private void CheckForUnexpectedFiles(HashSet<string> allowedFiles, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			foreach (var fullFilePath in fileStorage.EnumerateFiles())
			{
				if (!allowedFiles.Contains(fullFilePath))
				{
					inconsistenciesHandler(new UnexpectedFileInconsistency(fullFilePath));
				}
			}
		}

		private void CheckStorageFolder(FilePath folderPath, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			if (!fileStorage.FolderExists(folderPath))
			{
				inconsistenciesHandler(new MissingFolderInconsistency(fileStorage.GetFullPath(folderPath)));
			}
		}

		private void CheckStorageFile(FilePath filePath, bool checkContent, long size, uint checksum, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			var fullPath = fileStorage.GetFullPath(filePath);

			if (!fileStorage.FileExists(filePath))
			{
				inconsistenciesHandler(new MissingFileInconsistency(fullPath));
				return;
			}

			fileStorage.CheckFile(filePath, inconsistenciesHandler);

			if (checkContent)
			{
				CheckFileContent(fullPath, size, checksum, inconsistenciesHandler);
			}
		}

		private void CheckFileContent(string filePath, long size, uint checksum, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			var actualSize = CalculateFileSize(filePath);
			if (actualSize != size)
			{
				inconsistenciesHandler(new IncorrectFileSizeInconsistency(filePath, size, actualSize));
			}

			var actualChecksum = CalculateFileChecksum(filePath);
			if (actualChecksum != checksum)
			{
				inconsistenciesHandler(new IncorrectFileChecksumInconsistency(filePath, checksum, actualChecksum));
			}
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
