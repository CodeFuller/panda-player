using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface ISongsRepository
	{
		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, UpdatedSongProperties updatedProperties, CancellationToken cancellationToken);

		Task UpdateSongLastPlayback(SongModel song, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);
	}
}
