using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ContentUpdate
{
	public interface ILibraryContentUpdater
	{
		Task SetSongsRating(IEnumerable<Song> songs, Rating newRating);

		Task UpdateSongs(IEnumerable<Song> songs, UpdatedSongProperties updatedProperties);

		Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties);

		Task DeleteSong(Song song);

		Task DeleteDisc(Disc disc);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task ChangeSongUri(Song song, Uri newSongUri);
	}
}
