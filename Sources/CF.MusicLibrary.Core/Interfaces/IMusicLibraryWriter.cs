using System;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IMusicLibraryWriter
	{
		Task AddSong(Song song, string sourceSongFileName);

		Task UpdateSong(Song song, UpdatedSongProperties updatedProperties);

		Task ChangeSongUri(Song song, Uri newSongUri);

		Task DeleteSong(Song song, DateTime deleteTime);

		Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task DeleteDisc(Disc disc);

		Task SetDiscCoverImage(Disc disc, ImageInfo imageInfo);

		Task AddSongPlayback(Song song, DateTime playbackTime);

		Task FixSongTagData(Song song);
	}
}
