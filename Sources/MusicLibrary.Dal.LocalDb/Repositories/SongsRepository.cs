using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class SongsRepository : ISongsRepository
	{
		private readonly IDataStorage dataStorage;

		private readonly DiscLibrary discLibrary;

		public SongsRepository(IDataStorage dataStorage, DiscLibrary discLibrary)
		{
			this.dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<IReadOnlyCollection<SongModel>> GetSongs(IEnumerable<ItemId> songIds, CancellationToken cancellationToken)
		{
			var songs = new List<SongModel>();
			foreach (var id in songIds)
			{
				// TBD: Check error handling in caller (Loading songs playlist).
				var song = discLibrary.Songs.Single(s => s.Id.ToItemId() == id);

				// TBD: We can optimize this and re-use disc objects
				var disc = discLibrary.Discs.Single(d => d.Id == song.DiscId).ToModel(dataStorage);

				songs.Add(song.ToModel(disc, dataStorage));
			}

			return Task.FromResult<IReadOnlyCollection<SongModel>>(songs);
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
