using System.Threading.Tasks;

namespace PandaPlayer.Services.Media
{
	public interface ISongMediaInfoProvider
	{
		Task<SongMediaInfo> GetSongMediaInfo(string songFileName);
	}
}
