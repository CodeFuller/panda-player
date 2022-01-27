using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface ISongsService
	{
		Task CreateSong(SongModel song, Stream songContent, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken);

		internal Task<SongModel> GetSongWithPlaybacks(ItemId songId, CancellationToken cancellationToken);

		Task UpdateSong(SongModel song, Action<SongModel> updateAction, CancellationToken cancellationToken);

		Task AddSongPlayback(SongModel song, DateTimeOffset playbackTime, CancellationToken cancellationToken);

		Task DeleteSong(SongModel song, string deleteComment, CancellationToken cancellationToken);

		internal Task DeleteSong(SongModel song, DateTimeOffset deleteTime, string deleteComment, CancellationToken cancellationToken);
	}
}
