using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ContentUpdate
{
	public class LibraryContentUpdater : ILibraryContentUpdater
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IClock clock;

		public LibraryContentUpdater(IMusicLibrary musicLibrary, IClock clock)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
		}

		public async Task SetSongsRating(IEnumerable<Song> songs, Rating newRating)
		{
			var songsList = songs.ToList();
			foreach (var song in songsList)
			{
				song.Rating = newRating;
			}

			await UpdateSongs(songsList, UpdatedSongProperties.Rating);
		}

		public async Task UpdateSongs(IEnumerable<Song> songs, UpdatedSongProperties updatedProperties)
		{
			foreach (var song in songs)
			{
				await musicLibrary.UpdateSong(song, updatedProperties);
			}
		}

		public Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties)
		{
			return musicLibrary.UpdateDisc(disc, updatedProperties);
		}

		public Task DeleteSong(Song song)
		{
			return musicLibrary.DeleteSong(song, clock.Now);
		}

		public Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			return musicLibrary.ChangeDiscUri(disc, newDiscUri);
		}

		public Task ChangeSongUri(Song song, Uri newSongUri)
		{
			return musicLibrary.ChangeSongUri(song, newSongUri);
		}
	}
}
