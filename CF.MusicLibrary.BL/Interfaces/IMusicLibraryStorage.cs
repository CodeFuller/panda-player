using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibraryStorage
	{
		Task AddSong(Song song, string songSourceFileName);

		Task DeleteSong(Song song);

		Task SetDiscCoverImage(Disc disc, string coverImageFileName);

		Task UpdateSongTagData(Song song, UpdatedSongProperties updatedProperties);

		Task FixSongTagData(Song song);

		Task<SongTagData> GetSongTagData(Song song);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song);

		Task<FileInfo> GetSongFile(Song song);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task ChangeSongUri(Song song, Uri newSongUri);

		Task CheckDataConsistency(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator);
	}
}
