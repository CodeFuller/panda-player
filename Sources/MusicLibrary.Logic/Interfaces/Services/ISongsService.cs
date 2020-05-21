using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Services
{
	public interface ISongsService
	{
		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task<string> GetSongFile(SongModel song, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken);

		Task AddSongPlayback(SongModel song, DateTimeOffset playbackDateTime, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);
	}
}
