using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibraryWriter
	{
		Task AddSong(Song song, string songSourceFileName);

		Task DeleteSong(Song song);

		Task DeleteDisc(Disc disc);

		Task SetDiscCoverImage(Disc disc, string coverImageFileName);

		Task AddSongPlayback(Song song, DateTime playbackTime);

		Task UpdateSongTagData(Song song, SongTagData tagData);

		Task FixSongTagData(Song song);
	}
}
