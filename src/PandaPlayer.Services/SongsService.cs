using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class SongsService : ISongsService
	{
		private readonly ISongsRepository songsRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IClock clock;

		private readonly ILogger<SongsService> logger;

		private readonly IEqualityComparer<ArtistModel> artistsComparer = new ArtistEqualityComparer();

		private readonly IEqualityComparer<GenreModel> genresComparer = new GenreEqualityComparer();

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

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

			// Adding to repository should be performed after adding to the storage, because later updates song properties (Duration, BitRate, Size, Checksum).
			await songsRepository.CreateSong(song, cancellationToken);

			DiscLibrary.AddSong(song);
		}

		public Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.TryGetSongs(songIds));
		}

		public async Task UpdateSong(SongModel song, Action<SongModel> updateAction, CancellationToken cancellationToken)
		{
			var currentSong = song.CloneShallow();

			updateAction(song);

			if (!song.IsDeleted)
			{
				if (song.TreeTitle != currentSong.TreeTitle)
				{
					await storageRepository.UpdateSongTreeTitle(currentSong, song, cancellationToken);
				}

				// Checking if storage data (tags) must be updated.
				if (song.TrackNumber != currentSong.TrackNumber || song.Title != currentSong.Title ||
				    !artistsComparer.Equals(song.Artist, currentSong.Artist) || !genresComparer.Equals(song.Genre, currentSong.Genre))
				{
					await storageRepository.UpdateSong(song, cancellationToken);
				}
			}

			// Update in repository should be performed after update in the storage, because later updates song checksum.
			await songsRepository.UpdateSong(song, cancellationToken);
		}

		public async Task UpdateSongContent(SongModel song, CancellationToken cancellationToken)
		{
			logger.LogInformation($"Updating content for song '{song.TreeTitle}' ...");

			await storageRepository.UpdateSongContent(song, cancellationToken);

			await songsRepository.UpdateSong(song, cancellationToken);
		}

		public Task AddSongPlayback(SongModel song, DateTimeOffset playbackTime, CancellationToken cancellationToken)
		{
			song.AddPlayback(playbackTime);
			return songsRepository.UpdateSongLastPlayback(song, cancellationToken);
		}

		public Task DeleteSong(SongModel song, string deleteComment, CancellationToken cancellationToken)
		{
			return ((ISongsService)this).DeleteSong(song, clock.Now, deleteComment, cancellationToken);
		}

		async Task ISongsService.DeleteSong(SongModel song, DateTimeOffset deleteTime, string deleteComment, CancellationToken cancellationToken)
		{
			logger.LogInformation($"Deleting song '{song.TreeTitle}' ...");

			await storageRepository.DeleteSong(song, cancellationToken);

			song.MarkAsDeleted(deleteTime, deleteComment);
			await songsRepository.UpdateSong(song, cancellationToken);
		}
	}
}
