using System.Threading.Tasks;

namespace MusicLibrary.Core.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
