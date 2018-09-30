using System.Threading.Tasks;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.LastFM
{
	public interface IScrobbler
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
