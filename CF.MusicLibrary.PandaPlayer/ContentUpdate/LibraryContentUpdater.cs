using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ContentUpdate
{
	public class LibraryContentUpdater : ILibraryContentUpdater
	{
		private readonly IMusicLibrary musicLibrary;

		public LibraryContentUpdater(IMusicLibrary musicLibrary)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}

			this.musicLibrary = musicLibrary;
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

		public async Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties)
		{
			await musicLibrary.UpdateDisc(disc, updatedProperties);
		}

		public async Task DeleteDisc(Disc disc)
		{
			await musicLibrary.DeleteDisc(disc);
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await musicLibrary.ChangeDiscUri(disc, newDiscUri);
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await musicLibrary.ChangeSongUri(song, newSongUri);
		}
	}
}
