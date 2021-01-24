using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Facades;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
{
	internal class SongsService : ISongsService
	{
		private readonly ISongsRepository songsRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IClock clock;

		private readonly ILogger<SongsService> logger;

		private readonly IEqualityComparer<ArtistModel> artistsComparer = new ArtistEqualityComparer();

		private readonly IEqualityComparer<GenreModel> genresComparer = new GenreEqualityComparer();

		public SongsService(ISongsRepository songsRepository, IStorageRepository storageRepository, IClock clock, ILogger<SongsService> logger)
		{
			this.songsRepository = songsRepository ?? throw new ArgumentNullException(nameof(songsRepository));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateSong(SongModel song, Stream songContent, CancellationToken cancellationToken)
		{
			await storageRepository.AddSong(song, songContent, cancellationToken);

			// Adding to repository should be performed after adding to the storage, because later updates song checksum.
			await songsRepository.CreateSong(song, cancellationToken);
		}

		public Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			return songsRepository.GetSongs(songIds, cancellationToken);
		}

		public async Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			// Reading current song properties for several reasons:
			// 1. We need to understand which properties were actually changed.
			//    It defines whether song content will be updated (for correct tag values) and whether file in the storage must be renamed.
			// 2. Avoid overwriting of changes made by another clients.
			var currentSong = await songsRepository.GetSong(song.Id, cancellationToken);

			if (song.TreeTitle != currentSong.TreeTitle)
			{
				await storageRepository.UpdateSongTreeTitle(currentSong, song, cancellationToken);
			}

			// Checking if storage data (tags) must be updated.
			if (song.Disc.AlbumTitle != currentSong.Disc.AlbumTitle || song.Disc.Year != currentSong.Disc.Year ||
				song.TrackNumber != currentSong.TrackNumber || song.Title != currentSong.Title ||
			    !artistsComparer.Equals(song.Artist, currentSong.Artist) || !genresComparer.Equals(song.Genre, currentSong.Genre))
			{
				await storageRepository.UpdateSong(song, cancellationToken);
			}

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
			return ((ISongsService)this).DeleteSong(song, clock.Now, cancellationToken);
		}

		async Task ISongsService.DeleteSong(SongModel song, DateTimeOffset deleteTime, CancellationToken cancellationToken)
		{
			logger.LogInformation($"Deleting song '{song.TreeTitle}' ...");

			await storageRepository.DeleteSong(song, cancellationToken);

			song.DeleteDate = deleteTime;
			song.BitRate = null;
			song.Size = null;
			song.Checksum = null;

			await songsRepository.UpdateSong(song, cancellationToken);
		}
	}
}
