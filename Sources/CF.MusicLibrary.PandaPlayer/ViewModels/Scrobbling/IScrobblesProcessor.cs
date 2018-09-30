using System.Threading.Tasks;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public interface IScrobblesProcessor
	{
		Task ProcessScrobble(TrackScrobble scrobble, IScrobbler scrobbler);
	}
}
