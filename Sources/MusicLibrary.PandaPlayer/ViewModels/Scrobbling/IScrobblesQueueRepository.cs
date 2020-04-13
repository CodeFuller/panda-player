using System.Collections.Generic;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public interface IScrobblesQueueRepository : IGenericDataRepository<Queue<TrackScrobble>>
	{
	}
}
