using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibraryReader
	{
		Task<DiscLibrary> Load();

		Task<IEnumerable<Disc>> GetDiscsAsync();

		Task<SongTagData> GetSongTagData(Song song);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song);

		Task<FileInfo> GetSongFile(Song song);

		Task<bool> CheckSongContent(Song song);
	}
}
