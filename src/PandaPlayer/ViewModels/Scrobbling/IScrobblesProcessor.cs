using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.LastFM.Interfaces;
using PandaPlayer.LastFM.Objects;

namespace PandaPlayer.ViewModels.Scrobbling
{
	public interface IScrobblesProcessor
	{
		Task ProcessScrobble(TrackScrobble scrobble, IScrobbler scrobbler, CancellationToken cancellationToken);
	}
}
