using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface ISongsRepository
	{
		Task<SongModel> GetSong(ItemId songId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task UpdateSongLastPlayback(SongModel song, CancellationToken cancellationToken);
	}
}
