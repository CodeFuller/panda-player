using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class FileStorageOrganizer : IFileStorageOrganizer
	{
		private readonly IFolderProvider folderProvider;

		public FileStorageOrganizer(IFolderProvider folderProvider)
		{
			this.folderProvider = folderProvider ?? throw new ArgumentNullException(nameof(folderProvider));
		}

		public FilePath GetSongFilePath(SongModel song)
		{
			return GetDiscPath(song.Disc)
				.Add(song.TreeTitle);
		}

		public FilePath GetDiscImagePath(DiscImageModel image)
		{
			return GetDiscPath(image.Disc)
				.Add(image.TreeTitle);
		}

		public FilePath GetFolderPath(ShallowFolderModel folder)
		{
			return GetFolderPathInternal(folder);
		}

		private FilePath GetDiscPath(DiscModel disc)
		{
			return GetFolderPathInternal(disc.Folder)
				.Add(disc.TreeTitle);
		}

		private FilePath GetFolderPathInternal(ShallowFolderModel folder)
		{
			var path = new List<string>();

			for (var currentFolderId = folder.Id; ;)
			{
				var currentFolder = folderProvider.GetFolder(currentFolderId);
				if (currentFolder.IsRoot)
				{
					break;
				}

				path.Add(currentFolder.Name);
				currentFolderId = currentFolder.ParentFolderId;
			}

			path.Reverse();
			return new FilePath(path);
		}
	}
}
