using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface ISongsService
	{
		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, CancellationToken cancellationToken);

		Task AddSongPlayback(SongModel song, DateTimeOffset playbackTime, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, CancellationToken cancellationToken);

		internal Task DeleteSong(SongModel song, DateTimeOffset deleteTime, CancellationToken cancellationToken);
	}
}
