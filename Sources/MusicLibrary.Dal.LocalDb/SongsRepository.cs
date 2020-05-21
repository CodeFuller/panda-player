using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	internal class SongsRepository : ISongsRepository
	{
		private readonly DiscLibrary discLibrary;

		public SongsRepository(DiscLibrary discLibrary)
		{
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
				var disc = discLibrary.Discs.Single(d => d.Id == song.DiscId).ToModel();

				songs.Add(song.ToModel(disc));
			}

			return Task.FromResult<IReadOnlyCollection<SongModel>>(songs);
		}

		public Task<string> GetSongFile(SongModel song, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task UpdateSong(SongModel song, UpdatedSongPropertiesModel updatedProperties, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task AddSongPlayback(SongModel song, DateTimeOffset playbackDateTime, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task DeleteSong(SongModel song, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
