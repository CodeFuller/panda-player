using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

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
				SongTagger.SetTagData(destFileName, new SongTagData
				{
					Artist = song.Artist?.Name,
					Album = song.Disc.AlbumTitle,
					Year = song.Year,
					Genre = song.Genre?.Name,
					Track = song.TrackNumber,
					Title = song.Title,
				});
				fileSystemFacade.SetReadOnlyAttribute(destFileName);
			});
		}

		public async Task DeleteSong(Song song)
		{
			await Task.Run(() =>
			{
				fileSystemFacade.DeleteFile(UriToFilesystemPath(song.Uri));
			});
		}

		public async Task DeleteDisc(Disc disc)
		{
			await Task.Run(() =>
			{
				var discDirectory = UriToFilesystemPath(disc.Uri);
				var coverImageFileName = Path.Combine(discDirectory, DiscCoverFileName);
				if (fileSystemFacade.FileExists(coverImageFileName))
				{
					fileSystemFacade.DeleteFile(coverImageFileName);
				}

				if (fileSystemFacade.DirectoryIsEmpty(discDirectory))
				{
					throw new InvalidOperationException($"Could not delete disc directory because it's not empty: '{discDirectory}'");
				}
				fileSystemFacade.DeleteDirectory(discDirectory);
			});
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			string discDirectory = UriToFilesystemPath(disc.Uri);
			string destFileName = Path.Combine(discDirectory, DiscCoverFileName);

			await Task.Run(() => StoreFile(coverImageFileName, destFileName, true));
		}

		public async Task<bool> CheckSongContent(Song song)
		{
			var songFileName = UriToFilesystemPath(song.Uri);
			return await Task.Run(() => File.Exists(songFileName));
		}

		public async Task UpdateSongTagData(Song song, SongTagData tagData)
		{
			await Task.Run(() => SongTagger.SetTagData(UriToFilesystemPath(song.Uri), tagData));
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
	}
}
