using System;
using System.Collections.Generic;
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

		public async Task UpdateSongs(IEnumerable<Song> songs, UpdatedSongProperties updatedProperties)
		{
			foreach (var song in songs)
			{
				await musicLibrary.UpdateSong(song, updatedProperties);
			}
		}

		public async Task DeleteDisc(Disc disc)
		{
			await musicLibrary.DeleteDisc(disc);
		}
	}
}
