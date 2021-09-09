using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class SongsRepository : ISongsRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		private readonly IContentUriProvider contentUriProvider;

		public SongsRepository(IDbContextFactory<MusicDbContext> contextFactory, IContentUriProvider contentUriProvider)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.contentUriProvider = contentUriProvider ?? throw new ArgumentNullException(nameof(contentUriProvider));
		}

		public async Task CreateSong(SongModel song, CancellationToken cancellationToken)
		{
			var songEntity = song.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.Songs.AddAsync(songEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			song.Id = songEntity.Id.ToItemId();
		}

		public async Task<SongModel> GetSong(ItemId songId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var id = songId.ToInt32();
			var songEntity = await GetSongsQueryable(context)
				.Where(song => song.Id == id)
				.SingleAsync(cancellationToken);

			var discModel = songEntity.Disc.ToModel(contentUriProvider);
			return discModel.AllSongs.Single(song => song.Id == songId);
		}

		public async Task<SongModel> GetSongWithPlaybacks(ItemId songId, CancellationToken cancellationToken)
		{
			// Currently this method is used only for IT purposes.
			// We load song indirectly via loading disc data,
			// because we need full graph of objects for comparison.
			await using var context = contextFactory.CreateDbContext();

			var id = songId.ToInt32();

			var songDisc = await context.Discs
				.Include(disc => disc.Folder).ThenInclude(folder => folder.AdviseGroup)
				.Include(disc => disc.AdviseGroup)
				.Include(disc => disc.AdviseSet)
				.Include(disc => disc.Songs).ThenInclude(song => song.Artist)
				.Include(disc => disc.Songs).ThenInclude(song => song.Genre)
				.Include(disc => disc.Songs).ThenInclude(song => song.Playbacks)
				.Include(disc => disc.Images)
				.Where(disc => disc.Songs.Any(song => song.Id == id))
				.SingleAsync(cancellationToken);

			var discModel = songDisc.ToModel(contentUriProvider);
			return discModel.AllSongs.Single(song => song.Id == songId);
		}

		public async Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			var songIdsList = songIds.ToList();
			var ids = songIdsList.Select(id => id.ToInt32()).ToList();

			await using var context = contextFactory.CreateDbContext();

			var songs = await GetSongsQueryable(context)
				.Where(song => ids.Contains(song.Id))
				.ToListAsync(cancellationToken);

			var discEntities = songs
				.Select(song => song.Disc)
				.Distinct()
				.ToList();

			var folderModels = discEntities
				.Select(disc => disc.Folder)
				.Distinct()
				.Select(folder => folder.ToShallowModel())
				.ToDictionary(folder => folder.Id, folder => folder);

			var songModels = discEntities
				.Select(disc => disc.ToModel(folderModels[disc.Folder.Id.ToItemId()], contentUriProvider))
				.SelectMany(disc => disc.AllSongs)
				.ToDictionary(song => song.Id, song => song);

			return songIdsList
				.Where(id => songModels.ContainsKey(id))
				.Select(id => songModels[id])
				.ToList();
		}

		public async Task UpdateSong(SongModel song, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var songEntity = await FindSong(context, song.Id, cancellationToken);

			var updatedEntity = song.ToEntity();
			context.Entry(songEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateSongLastPlayback(SongModel song, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();
			var songEntity = await FindSong(context, song.Id, cancellationToken, includePlaybacks: true);

			SyncSongPlaybacks(song, songEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		private static void SyncSongPlaybacks(SongModel model, SongEntity entity)
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

		private static async Task<SongEntity> FindSong(MusicDbContext context, ItemId id, CancellationToken cancellationToken, bool includePlaybacks = false)
		{
			IQueryable<SongEntity> queryable = context.Songs;

			if (includePlaybacks)
			{
				queryable = queryable.Include(s => s.Playbacks);
			}

			var entityId = id.ToInt32();
			return await queryable
				.SingleAsync(s => s.Id == entityId, cancellationToken);
		}

		private static IQueryable<SongEntity> GetSongsQueryable(MusicDbContext context)
		{
			return context.Songs
				.Include(song => song.Disc).ThenInclude(disc => disc.Songs)
				.Include(song => song.Disc).ThenInclude(disc => disc.Images)
				.Include(song => song.Disc).ThenInclude(disc => disc.Folder).ThenInclude(folder => folder.AdviseGroup)
				.Include(song => song.Disc).ThenInclude(disc => disc.AdviseGroup)
				.Include(song => song.Disc).ThenInclude(disc => disc.AdviseSet)
				.Include(song => song.Artist)
				.Include(song => song.Genre);
		}
	}
}
