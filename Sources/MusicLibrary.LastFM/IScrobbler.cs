using System.Threading.Tasks;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM
{
	public interface IScrobbler
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
