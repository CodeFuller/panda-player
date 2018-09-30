using System.Collections.Generic;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public interface IScrobblesQueueRepository : IGenericDataRepository<Queue<TrackScrobble>>
	{
	}
}
