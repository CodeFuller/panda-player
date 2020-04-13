using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.Core.Media;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.Core.Interfaces
{
	public interface IMusicLibraryReader
	{
		/// <summary>
		/// Loads all library discs including deleted.
		/// </summary>
		Task<IEnumerable<Disc>> LoadDiscs();

		Task<DiscLibrary> LoadLibrary();

		Task<string> GetSongFile(Song song);

		Task<SongTagData> GetSongTagData(Song song);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song);

		Task<string> GetDiscCoverImage(Disc disc);

		Task CheckStorage(DiscLibrary library, IUriCheckScope checkScope, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues);

		Task CheckStorageChecksums(DiscLibrary library, ICheckScope checkScope, ILibraryStorageInconsistencyRegistrator registrator, bool fixFoundIssues);
	}
}
