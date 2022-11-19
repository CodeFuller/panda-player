using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Interfaces;

namespace PandaPlayer.Dal.LocalDb.Internal
{
	internal class FileStorageOrganizer : IFileStorageOrganizer
	{
		public FilePath GetDiscFolderPath(DiscModel disc)
		{
			return GetDiscPath(disc);
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

		public FilePath GetFolderPath(FolderModel folder)
		{
			return GetFolderPathInternal(folder);
		}

		private static FilePath GetDiscPath(DiscModel disc)
		{
			return GetFolderPathInternal(disc.Folder)
				.Add(disc.TreeTitle);
		}

		private static FilePath GetFolderPathInternal(FolderModel folder)
		{
			return new FilePath(folder.PathNames);
		}
	}
}
