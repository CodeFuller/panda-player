using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
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

		public Task<string> GetSongFile(SongModel song, CancellationToken cancellationToken)
		{
			return songsRepository.GetSongFile(song, cancellationToken);
		}

		public Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken)
		{
			return songsRepository.UpdateSong(song, updatedProperties, cancellationToken);
		}

		public Task AddSongPlayback(SongModel song, DateTimeOffset playbackDateTime, CancellationToken cancellationToken)
		{
			return songsRepository.AddSongPlayback(song, playbackDateTime, cancellationToken);
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			return songsRepository.DeleteSong(song, cancellationToken);
		}
	}
}
