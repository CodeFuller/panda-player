using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IStorageRepository
	{
		Task UpdateDiscTreeTitle(DiscModel oldDisc, DiscModel newDisc, CancellationToken cancellationToken);

		Task UpdateSongTreeTitle(SongModel oldSong, SongModel newSong, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);

		Task AddDiscImage(DiscImageModel image, Stream imageContent, CancellationToken cancellationToken);

		Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken);

		Task DeleteFolder(ShallowFolderModel folder, CancellationToken cancellationToken);
	}
}
