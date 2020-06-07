using System.Threading.Tasks;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM.Interfaces
{
	public interface ILastFMApiClient
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
