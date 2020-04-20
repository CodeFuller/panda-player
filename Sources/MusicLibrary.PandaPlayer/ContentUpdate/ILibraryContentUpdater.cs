using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ContentUpdate
{
	public interface ILibraryContentUpdater
	{
		Task SetSongsRating(IEnumerable<Song> songs, Rating newRating);

		Task UpdateSongs(IEnumerable<Song> songs, UpdatedSongProperties updatedProperties);

		Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties);

		Task DeleteSong(Song song);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task ChangeSongUri(Song song, Uri newSongUri);
	}
}
