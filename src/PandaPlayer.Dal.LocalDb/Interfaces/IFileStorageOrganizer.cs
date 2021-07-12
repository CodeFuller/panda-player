using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Internal;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	internal interface IFileStorageOrganizer
	{
		FilePath GetDiscFolderPath(DiscModel disc);

		FilePath GetSongFilePath(SongModel song);

		FilePath GetDiscImagePath(DiscImageModel image);

		FilePath GetFolderPath(ShallowFolderModel folder);
	}
}
