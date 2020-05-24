using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Dal
{
	public interface ISongsRepository
	{
		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken);

		Task UpdateSongPlaybacks(SongModel song, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);
	}
}
