using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services
{
	internal class SongsService : ISongsService
	{
		private readonly ISongsRepository songsRepository;

		public SongsService(ISongsRepository songsRepository)
		{
			this.songsRepository = songsRepository ?? throw new ArgumentNullException(nameof(songsRepository));
		}

		public Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			return songsRepository.GetSongs(songIds, cancellationToken);
		}

		public Task UpdateSong(SongModel song, UpdatedSongProperties updatedProperties, CancellationToken cancellationToken)
		{
			return songsRepository.UpdateSong(song, updatedProperties, cancellationToken);
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
