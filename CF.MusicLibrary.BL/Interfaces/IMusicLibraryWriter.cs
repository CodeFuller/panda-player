using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibraryWriter
	{
		Task AddSong(Song song, string songSourceFileName);

		Task UpdateSong(Song song, UpdatedSongProperties updatedProperties);

		Task DeleteSong(Song song, DateTime deleteTime);

		Task ChangeSongUri(Song song, Uri newSongUri);

		Task DeleteDisc(Disc disc);

		Task SetDiscCoverImage(Disc disc, string coverImageFileName);

		Task AddSongPlayback(Song song, DateTime playbackTime);

		Task FixSongTagData(Song song);
	}
}
