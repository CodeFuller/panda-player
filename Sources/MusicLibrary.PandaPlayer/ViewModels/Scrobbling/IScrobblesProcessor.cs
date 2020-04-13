using System.Threading.Tasks;
using MusicLibrary.LastFM;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public interface IScrobblesProcessor
	{
		Task ProcessScrobble(TrackScrobble scrobble, IScrobbler scrobbler);
	}
}
