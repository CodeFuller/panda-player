using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Services
{
	public interface ISongsService
	{
		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken);

		Task AddSongPlayback(SongModel song, DateTimeOffset playbackTime, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);
	}
}
