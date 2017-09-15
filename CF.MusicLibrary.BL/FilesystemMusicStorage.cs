using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL
{
	public class FileSystemMusicStorage : IMusicLibraryStorage
	{
		private static string DiscCoverFileName = "cover.jpg";

		private readonly IFileSystemFacade fileSystemFacade;
		private readonly string libraryRootDirectory;

		private readonly ISongTagger songTagger;
		private ISongTagger SongTagger
		{
			get
			{
				if (songTagger == null)
				{
					throw new InvalidOperationException($"SongTagger is not bootstrapped");
				}

				return songTagger;
			}
		}

		public FileSystemMusicStorage(IFileSystemFacade fileSystemFacade, string libraryRootDirectory) :
			this(fileSystemFacade, null, libraryRootDirectory)
		{
		}

		public FileSystemMusicStorage(IFileSystemFacade fileSystemFacade, ISongTagger songTagger, string libraryRootDirectory)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}
			if (libraryRootDirectory == null)
			{
				throw new ArgumentNullException(nameof(libraryRootDirectory));
			}

			this.fileSystemFacade = fileSystemFacade;
			this.songTagger = songTagger;
			this.libraryRootDirectory = libraryRootDirectory;
		}

		public async Task AddSong(Song song, string songSourceFileName)
		{
			await Task.Run(() =>
			{
				string destFileName = UriToFilesystemPath(song.Uri);
				fileSystemFacade.CreateDirectory(Path.GetDirectoryName(destFileName));

				StoreFile(songSourceFileName, destFileName, false);
				SongTagger.SetTagData(destFileName, FillSongTagData(song));
				fileSystemFacade.SetReadOnlyAttribute(destFileName);
			});
		}

		public async Task DeleteSong(Song song)
		{
			await Task.Run(() =>
			{
				var songFileName = UriToFilesystemPath(song.Uri);
				fileSystemFacade.ClearReadOnlyAttribute(songFileName);
				fileSystemFacade.DeleteFile(songFileName);

				var discDirectory = Path.GetDirectoryName(songFileName);
				var restDirectoryFiles = fileSystemFacade.EnumerateFiles(discDirectory).ToList();
				if (restDirectoryFiles.Count == 0 || restDirectoryFiles.Count == 1 && IsCoverImageFile(restDirectoryFiles.Single()))
				{
					if (restDirectoryFiles.Count == 1)
					{
						var coverImageFileName = restDirectoryFiles.Single();
						fileSystemFacade.ClearReadOnlyAttribute(coverImageFileName);
						fileSystemFacade.DeleteFile(coverImageFileName);
					}

					//	Deleting directories that became empty (disc, artist, category, ...)
					foreach (var currDirectoryPath in GetParentDirectoriesWithinStorage(discDirectory))
					{
						if (!fileSystemFacade.DirectoryIsEmpty(currDirectoryPath))
						{
							break;
						}
						fileSystemFacade.DeleteDirectory(currDirectoryPath);
					}
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

		private static bool IsCoverImageFile(string filePath)
		{
			return String.Equals(Path.GetFileName(filePath), DiscCoverFileName, StringComparison.OrdinalIgnoreCase);
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			string discDirectory = UriToFilesystemPath(disc.Uri);
			string destFileName = Path.Combine(discDirectory, DiscCoverFileName);

			await Task.Run(() => StoreFile(coverImageFileName, destFileName, true));
		}

		public async Task UpdateSongTagData(Song song, UpdatedSongProperties updatedProperties)
		{
			await Task.Run(() =>
			{
				string songFileName = UriToFilesystemPath(song.Uri);
				fileSystemFacade.ClearReadOnlyAttribute(songFileName);
				SongTagger.SetTagData(songFileName, FillSongTagData(song));
				fileSystemFacade.SetReadOnlyAttribute(songFileName);
			});
		}

		public async Task FixSongTagData(Song song)
		{
			await Task.Run(() => SongTagger.FixTagData(UriToFilesystemPath(song.Uri)));
		}

		public async Task<SongTagData> GetSongTagData(Song song)
		{
			return await Task.Run(() => SongTagger.GetTagData(UriToFilesystemPath(song.Uri)));
		}

		public async Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song)
		{
			return await Task.Run(() => SongTagger.GetTagTypes(UriToFilesystemPath(song.Uri)));
		}

		public Task<FileInfo> GetSongFile(Song song)
		{
			return Task.FromResult(new FileInfo(UriToFilesystemPath(song.Uri)));
		}

		public async Task CheckDataConsistency(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator)
		{
			await Task.Run(() =>
			{
				//	Checking that all song files exist and have read-only attribute set.
				foreach (var song in library.Songs)
				{
					var songFileName = UriToFilesystemPath(song.Uri);
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
			HashSet<string> expectedFileNames = new HashSet<string>(library.Songs.Select(s => UriToFilesystemPath(s.Uri)));

			foreach (var disc in library)
			{
				var discDirectory = UriToFilesystemPath(disc.Uri);
				expectedFileNames.Add(Path.Combine(discDirectory, DiscCoverFileName));
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
					registrator.RegisterInconsistency_LibraryData(Current($"Detected unexpected file within the storage: {fileName}"));
				}
			}

			foreach (string directory in Directory.EnumerateDirectories(libraryRootDirectory, "*.*", SearchOption.AllDirectories))
			{
				if (!expectedDirectories.Contains(directory) &&
					!skipList.Any(s => directory.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
				{
					registrator.RegisterInconsistency_LibraryData(Current($"Detected unexpected directory within the storage: {directory}"));
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
	}
}
