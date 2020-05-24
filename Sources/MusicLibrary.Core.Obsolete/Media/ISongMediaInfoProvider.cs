using System.Threading.Tasks;

namespace MusicLibrary.Core.Obsolete.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
