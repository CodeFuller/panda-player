using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public class FileSystemMusicStorage : IMusicLibraryStorage
	{
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly IDiscArtFileStorage discArtFileStorage;
		private readonly string libraryRootDirectory;
		private readonly ISongTagger songTagger;

		public FileSystemMusicStorage(IFileSystemFacade fileSystemFacade, ISongTagger songTagger, IDiscArtFileStorage discArtFileStorage, string libraryRootDirectory)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (songTagger == null)
			{
				throw new ArgumentNullException(nameof(songTagger));
			}
			if (discArtFileStorage == null)
			{
				throw new ArgumentNullException(nameof(discArtFileStorage));
			}
			if (libraryRootDirectory == null)
			{
				throw new ArgumentNullException(nameof(libraryRootDirectory));
			}

			this.fileSystemFacade = fileSystemFacade;
			this.songTagger = songTagger;
			this.discArtFileStorage = discArtFileStorage;
			this.libraryRootDirectory = libraryRootDirectory;
		}

		public async Task AddSong(Song song, string songSourceFileName)
		{
			await Task.Run(() =>
			{
				string destFileName = GetSongFileName(song);
				fileSystemFacade.CreateDirectory(Path.GetDirectoryName(destFileName));

				StoreFile(songSourceFileName, destFileName, false);
				songTagger.SetTagData(destFileName, FillSongTagData(song));
				fileSystemFacade.SetReadOnlyAttribute(destFileName);
			});
		}

		public async Task DeleteSong(Song song)
		{
			await Task.Run(() =>
			{
				var songFileName = GetSongFileName(song);
				Application.Logger.WriteInfo($"Deleting song file '{songFileName}'");
				DeleteFile(songFileName);

				var discDirectory = Path.GetDirectoryName(songFileName);
				var restDirectoryFiles = fileSystemFacade.EnumerateFiles(discDirectory).ToList();
				Application.Logger.WriteDebug($"Rest directory files: {restDirectoryFiles.Count}");
				if (restDirectoryFiles.Count > 1)
				{
					return;
				}

				if (restDirectoryFiles.Count == 1)
				{
					var discCoverImageFileName = restDirectoryFiles.Single();
					if (!discArtFileStorage.IsCoverImageFile(discCoverImageFileName))
					{
						//	Rest directory file is not a cover image
						return;
					}

					Application.Logger.WriteInfo($"Deleting cover image file '{discCoverImageFileName}'");
					DeleteFile(discCoverImageFileName);
				}

				//	Deleting directories that became empty (disc, artist, category, ...)
				foreach (var currDirectoryPath in GetParentDirectoriesWithinStorage(discDirectory))
				{
					if (!fileSystemFacade.DirectoryIsEmpty(currDirectoryPath))
					{
						break;
					}
					Application.Logger.WriteInfo($"Deleting empty directory '{currDirectoryPath}'");
					fileSystemFacade.DeleteDirectory(currDirectoryPath);
				}
			});
		}

		private IEnumerable<string> GetParentDirectoriesWithinStorage(string startDirectoryPath)
		{
			string currDirectoryPath = startDirectoryPath;
			do
			{
				yield return currDirectoryPath;
				currDirectoryPath = Directory.GetParent(currDirectoryPath)?.FullName;
			}
			while (currDirectoryPath != null &&
						!String.Equals(Path.GetFullPath(currDirectoryPath), Path.GetFullPath(libraryRootDirectory), StringComparison.OrdinalIgnoreCase));
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			await Task.Run(() => discArtFileStorage.StoreDiscCoverImage(GetDiscDirecotry(disc), coverImageFileName));
		}

		public async Task<string> GetDiscCoverImage(Disc disc)
		{
			return await Task.FromResult(discArtFileStorage.GetDiscCoverImageFileName(GetDiscDirecotry(disc)));
		}

		public async Task UpdateSongTagData(Song song, UpdatedSongProperties updatedProperties)
		{
			await Task.Run(() =>
			{
				string songFileName = GetSongFileName(song);
				fileSystemFacade.ClearReadOnlyAttribute(songFileName);
				songTagger.SetTagData(songFileName, FillSongTagData(song));
				fileSystemFacade.SetReadOnlyAttribute(songFileName);
			});
		}

		public async Task FixSongTagData(Song song)
		{
			await Task.Run(() => songTagger.FixTagData(UriToFilesystemPath(song.Uri)));
		}

		public async Task<SongTagData> GetSongTagData(Song song)
		{
			return await Task.Run(() => songTagger.GetTagData(GetSongFileName(song)));
		}

		public async Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song)
		{
			return await Task.Run(() => songTagger.GetTagTypes(GetSongFileName(song)));
		}

		public Task<string> GetSongFile(Song song)
		{
			return Task.FromResult(GetSongFileName(song));
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await Task.Run(() =>
			{
				var oldFolderName = GetDiscDirecotry(disc);
				var newFolderName = UriToFilesystemPath(newDiscUri);
				fileSystemFacade.MoveDirectory(oldFolderName, newFolderName);
			});
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await Task.Run(() =>
			{
				var oldFileName = GetSongFileName(song);
				var newFileName = UriToFilesystemPath(newSongUri);
				fileSystemFacade.MoveFile(oldFileName, newFileName);
			});
		}

		public async Task CheckDataConsistency(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator)
		{
			await Task.Run(() =>
			{
				//	Checking that all song files exist and have read-only attribute set.
				foreach (var song in library.Songs)
				{
					var songFileName = GetSongFileName(song);
					if (!fileSystemFacade.FileExists(songFileName))
					{
						registrator.RegisterInconsistency_MissingSongData(song);
					}
					else if (!fileSystemFacade.GetReadOnlyAttribute(songFileName))
					{
						registrator.RegisterInconsistency_LibraryData($"Song file has no read-only attribute set: {songFileName}");
					}
				}

				CheckForUnexpectedFileSystemItems(library, registrator);
			});
		}

		private void CheckForUnexpectedFileSystemItems(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator)
		{
			HashSet<string> skipList = new HashSet<string>(new[] { @"d:\music\.sync" });

			HashSet<string> expectedDirectories = new HashSet<string>();
			HashSet<string> expectedFileNames = new HashSet<string>(library.Songs.Select(GetSongFileName));

			foreach (var disc in library.Discs)
			{
				var discDirectory = GetDiscDirecotry(disc);
				var discCoverFileName = discArtFileStorage.GetDiscCoverImageFileName(discDirectory);
				if (discCoverFileName != null)
				{
					expectedFileNames.Add(discCoverFileName);
				}
				foreach (var directory in GetParentDirectoriesWithinStorage(discDirectory))
				{
					expectedDirectories.Add(directory);
				}
			}

			foreach (string fileName in Directory.EnumerateFiles(libraryRootDirectory, "*.*", SearchOption.AllDirectories))
			{
				if (!expectedFileNames.Contains(fileName) &&
					!skipList.Any(s => fileName.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
				{
					registrator.RegisterInconsistency_LibraryData(FormattableStringExtensions.Current($"Detected unexpected file within the storage: {fileName}"));
				}
			}

			foreach (string directory in Directory.EnumerateDirectories(libraryRootDirectory, "*.*", SearchOption.AllDirectories))
			{
				if (!expectedDirectories.Contains(directory) &&
					!skipList.Any(s => directory.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
				{
					registrator.RegisterInconsistency_LibraryData(FormattableStringExtensions.Current($"Detected unexpected directory within the storage: {directory}"));
				}
			}
		}

		private string UriToFilesystemPath(Uri uri)
		{
			List<string> segments = new List<string>
			{
				libraryRootDirectory,
			};
			segments.AddRange(uri.SegmentsEx());

			return Path.Combine(segments.ToArray());
		}

		private void StoreFile(string srcFileName, string dstFileName, bool setReadOnly)
		{
			fileSystemFacade.CopyFile(srcFileName, dstFileName);
			if (setReadOnly)
			{
				fileSystemFacade.SetReadOnlyAttribute(dstFileName);
			}
			else
			{
				fileSystemFacade.ClearReadOnlyAttribute(dstFileName);
			}
		}

		private void DeleteFile(string fileName)
		{
			fileSystemFacade.ClearReadOnlyAttribute(fileName);
			fileSystemFacade.DeleteFile(fileName);
		}

		private static SongTagData FillSongTagData(Song song)
		{
			return new SongTagData
			{
				Artist = song.Artist?.Name,
				Album = song.Disc.AlbumTitle,
				Year = song.Year,
				Genre = song.Genre?.Name,
				Track = song.TrackNumber,
				Title = song.Title,
			};
		}

		private string GetSongFileName(Song song)
		{
			return UriToFilesystemPath(song.Uri);
		}

		private string GetDiscDirecotry(Disc disc)
		{
			return UriToFilesystemPath(disc.Uri);
		}
	}
}
