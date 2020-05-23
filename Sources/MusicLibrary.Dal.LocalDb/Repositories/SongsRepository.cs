using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

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
				.Include(song => song.Playbacks) // TBD: Can we avoid loading all playbacks? We actually can need this only for adding new playback (and showing playbacks history).
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

		public Task UpdateSongPlaybacks(SongModel song, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
