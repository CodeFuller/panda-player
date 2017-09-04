using System.Threading.Tasks;

namespace CF.MusicLibrary.BL.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
