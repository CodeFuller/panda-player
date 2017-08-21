using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public interface IScrobbler
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
