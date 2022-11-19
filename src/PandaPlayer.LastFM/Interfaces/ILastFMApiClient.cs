using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.LastFM.Objects;

namespace PandaPlayer.LastFM.Interfaces
{
	public interface ILastFMApiClient
	{
		Task UpdateNowPlaying(Track track, CancellationToken cancellationToken);

		Task Scrobble(TrackScrobble trackScrobble, CancellationToken cancellationToken);
	}
}
