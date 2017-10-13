using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IMusicLibraryStorage
	{
		Task AddSong(Song song, string songSourceFileName);

		Task DeleteSong(Song song);

		Task SetDiscCoverImage(Disc disc, string coverImageFileName);

		Task<string> GetDiscCoverImage(Disc disc);

		Task UpdateSongTagData(Song song, UpdatedSongProperties updatedProperties);

		Task FixSongTagData(Song song);

		Task<SongTagData> GetSongTagData(Song song);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song);

		Task<string> GetSongFile(Song song);

		Task ChangeDiscUri(Disc disc, Uri newDiscUri);

		Task ChangeSongUri(Song song, Uri newSongUri);

		Task CheckDataConsistency(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator);
	}
}
