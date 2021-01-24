using System.Threading.Tasks;

namespace MusicLibrary.Services.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
