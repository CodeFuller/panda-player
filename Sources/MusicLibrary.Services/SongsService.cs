using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Comparers;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
{
	internal class SongsService : ISongsService
	{
		private readonly ISongsRepository songsRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IEqualityComparer<ArtistModel> artistsComparer = new ArtistEqualityComparer();

		private readonly IEqualityComparer<GenreModel> genresComparer = new GenreEqualityComparer();

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
			// Reading current song properties for several reasons:
			// 1. We need to understand which properties were actually changed.
			//    It defines whether song content will be updated (for correct tag values) and whether file in the storage must be renamed.
			// 2. Avoid overwriting of changes made by another clients.
			var currentSong = await songsRepository.GetSong(song.Id, cancellationToken);

			if (song.TreeTitle != currentSong.TreeTitle)
			{
				await storageRepository.UpdateSongTreeTitle(song, currentSong.ContentUri, cancellationToken);
			}

			// Checking if storage data (tags) must be updated.
			if (song.TrackNumber != currentSong.TrackNumber || song.Title != currentSong.Title ||
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
			return songsRepository.DeleteSong(song, cancellationToken);
		}
	}
}
