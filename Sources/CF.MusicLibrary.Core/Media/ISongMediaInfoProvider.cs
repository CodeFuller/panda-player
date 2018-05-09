using System.Threading.Tasks;

namespace CF.MusicLibrary.Core.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
