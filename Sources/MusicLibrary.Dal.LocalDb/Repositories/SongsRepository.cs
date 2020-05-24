using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class SongsRepository : ISongsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IDataStorage dataStorage;

		public SongsRepository(IMusicLibraryDbContextFactory contextFactory, IDataStorage dataStorage)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
		}

		public async Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			var songIdsList = songIds.ToList();
			var ids = songIdsList.Select(id => id.ToInt32()).ToList();

			await using var context = contextFactory.Create();

			var songs = await context.Songs
				.Include(song => song.Disc).ThenInclude(disc => disc.Images)
				.Include(song => song.Artist)
				.Include(song => song.Genre)
				.Where(song => ids.Contains(song.Id))
				.ToListAsync(cancellationToken);

			var songModels = songs
				.Select(song => song.Disc)
				.Distinct()
				.Select(disc => disc.ToModel(dataStorage))
				.SelectMany(disc => disc.Songs)
				.ToDictionary(song => song.Id, song => song);

			return songIdsList
				.Select(id => songModels[id]) // TBD: Check error handling in caller (Loading songs playlist).
				.ToList();
		}

		public Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateSongLastPlayback(SongModel song, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var id = song.Id.ToInt32();
			var songEntity = await context.Songs
				.Include(s => s.Playbacks)
				.SingleAsync(s => s.Id == id, cancellationToken);

			SyncSongPlaybacks(song, songEntity);

			await context.SaveChangesAsync(cancellationToken);
		}

		public void SyncSongPlaybacks(SongModel model, SongEntity entity)
		{
			if (model.PlaybacksCount != entity.PlaybacksCount + 1)
			{
				throw new InvalidOperationException($"There is some discrepancy in song playbacks for song {entity.Id}: {model.PlaybacksCount} != {entity.PlaybacksCount + 1}");
			}

			if (model.LastPlaybackTime == null)
			{
				throw new InvalidOperationException("Last playback for the song is not set");
			}

			entity.PlaybacksCount = model.PlaybacksCount;
			entity.LastPlaybackTime = model.LastPlaybackTime;

			var playbackEntity = new PlaybackEntity
			{
				PlaybackTime = model.LastPlaybackTime.Value,
			};

			entity.Playbacks.Add(playbackEntity);
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
