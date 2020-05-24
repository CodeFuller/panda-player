using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
{
	internal class SongsService : ISongsService
	{
		private readonly ISongsRepository songsRepository;

		private readonly IStorageRepository storageRepository;

		public SongsService(ISongsRepository songsRepository, IStorageRepository storageRepository)
		{
			this.songsRepository = songsRepository ?? throw new ArgumentNullException(nameof(songsRepository));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
		}

		public Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			return songsRepository.GetSongs(songIds, cancellationToken);
		}

		public async Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			await storageRepository.UpdateSong(song, cancellationToken);

			// Update in repository should be performed after update in the storage, because later updates song checksum.
			await songsRepository.UpdateSong(song, cancellationToken);
		}

		public Task AddSongPlayback(SongModel song, DateTimeOffset playbackTime, CancellationToken cancellationToken)
		{
			song.AddPlayback(playbackTime);
			return songsRepository.UpdateSongLastPlayback(song, cancellationToken);
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			return songsRepository.DeleteSong(song, cancellationToken);
		}
	}
}
