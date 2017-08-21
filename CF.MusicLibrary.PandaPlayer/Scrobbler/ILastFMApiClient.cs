using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public interface ILastFMApiClient
	{
		Task OpenSession();

		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
