using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Internal;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	internal interface IFileStorageOrganizer
	{
		FilePath GetDiscFolderPath(DiscModel disc);

		FilePath GetSongFilePath(SongModel song);

		FilePath GetDiscImagePath(DiscImageModel image);

		FilePath GetFolderPath(ShallowFolderModel folder);
	}
}
