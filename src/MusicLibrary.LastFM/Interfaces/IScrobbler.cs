using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM.Interfaces
{
	public interface IScrobbler
	{
		Task UpdateNowPlaying(Track track, CancellationToken cancellationToken);

		Task Scrobble(TrackScrobble trackScrobble, CancellationToken cancellationToken);
	}
}
