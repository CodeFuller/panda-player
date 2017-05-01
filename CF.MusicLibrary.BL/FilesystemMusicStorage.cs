using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;

namespace CF.MusicLibrary.BL
{
	public class FilesystemMusicStorage : IMusicStorage
	{
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly string libraryRootDirectory;

		private readonly bool moveSongFiles;

		public FilesystemMusicStorage(IFileSystemFacade fileSystemFacade, string libraryRootDirectory, bool moveSongFiles)
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
			this.libraryRootDirectory = libraryRootDirectory;
			this.moveSongFiles = moveSongFiles;
		}

		public async Task AddSongAsync(string sourceFileName, Uri songUri)
		{
			await Task.Run(() =>
			{
				string destFileName = UriToFilesystemPath(songUri);
				fileSystemFacade.CreateDirectory(Path.GetDirectoryName(destFileName));

				if (moveSongFiles)
				{
					fileSystemFacade.MoveFile(sourceFileName, destFileName);
				}
				else
				{
					fileSystemFacade.CopyFile(sourceFileName, destFileName);
				}
			});
		}

		public async Task SetAlbumCoverImage(Uri albumUri, string coverImagePath)
		{
			string albumDirectoryPath = UriToFilesystemPath(albumUri);
			string destFileName = Path.Combine(albumDirectoryPath, Path.GetFileName(coverImagePath));

			await Task.Run(() => fileSystemFacade.CopyFile(coverImagePath, destFileName));
		}

		private string UriToFilesystemPath(Uri uri)
		{
			List<string> segments = new List<string>()
			{
				libraryRootDirectory,
			};
			segments.AddRange(uri.SegmentsEx());

			return Path.Combine(segments.ToArray());
		}
	}
}
