using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Inconsistencies.StorageInconsistencies;
using MusicLibrary.Dal.LocalDb.Inconsistencies.TagsInconsistencies;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal partial class StorageRepository : IStorageRepository, IContentUriProvider
	{
		public Task CheckStorage(LibraryCheckFlags checkFlags, IEnumerable<ShallowFolderModel> folders, IEnumerable<DiscModel> discs,
			IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			var knownFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			CheckFolders(folders, knownFolders, inconsistenciesHandler);

			var knownFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			CheckDiscsData(discs.ToList(), checkFlags, knownFolders, knownFiles, progress, inconsistenciesHandler);

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

		private void CheckDiscsData(IReadOnlyCollection<DiscModel> discs, LibraryCheckFlags checkFlags, HashSet<string> visitedFolders,
			HashSet<string> visitedFiles, IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			progress.SetOperationCost(discs.SelectMany(d => d.ActiveSongs).Count());

			var checkContent = checkFlags.HasFlag(LibraryCheckFlags.CheckContentConsistency);

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

					if (checkFlags.HasFlag(LibraryCheckFlags.CheckSongTagsConsistency))
					{
						CheckSongTags(song, inconsistenciesHandler);
					}

					progress.IncrementOperationProgress();
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

		private void CheckSongTags(SongModel song, Action<LibraryInconsistency> inconsistenciesHandler)
		{
			var songPath = storageOrganizer.GetSongFilePath(song);
			if (!fileStorage.FileExists(songPath))
			{
				// We can not get tags if song file is missing.
				// The "missing file" inconsistency is handled by storage checker.
				return;
			}

			var songFilePath = fileStorage.GetFullPath(songPath);
			var tagData = songTagger.GetTagData(songFilePath);

			if (tagData.Artist != song.Artist?.Name)
			{
				inconsistenciesHandler(new BadArtistTagInconsistency(song, tagData.Artist));
			}

			if (tagData.Album != song.Disc.AlbumTitle)
			{
				inconsistenciesHandler(new BadAlbumTagInconsistency(song, tagData.Album));
			}

			if (tagData.Year != song.Disc.Year)
			{
				inconsistenciesHandler(new BadYearTagInconsistency(song, tagData.Year));
			}

			if (tagData.Genre != song.Genre?.Name)
			{
				inconsistenciesHandler(new BadGenreTagInconsistency(song, tagData.Genre));
			}

			if (tagData.Track != song.TrackNumber)
			{
				inconsistenciesHandler(new BadTrackTagInconsistency(song, tagData.Track));
			}

			if (tagData.Title != song.Title)
			{
				inconsistenciesHandler(new BadTitleTagInconsistency(song, tagData.Title));
			}

			var tagTypes = songTagger.GetTagTypes(songFilePath);
			var unexpectedTagTypes = tagTypes.Except(new[] { SongTagType.Id3V1, SongTagType.Id3V2 }).ToList();

			if (unexpectedTagTypes.Any())
			{
				inconsistenciesHandler(new UnexpectedTagTypesInconsistency(song, unexpectedTagTypes));
			}
		}
	}
}
