using System.Collections.Generic;
using CF.Library.Core.Facades;
using CF.MusicLibrary.LastFM.Objects;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public class ScrobblesQueueRepository : JsonFileGenericRepository<Queue<TrackScrobble>>, IScrobblesQueueRepository
	{
		public ScrobblesQueueRepository(IFileSystemFacade fileSystemFacade, ILogger<ScrobblesQueueRepository> logger, string dataFileName)
			: base(fileSystemFacade, logger, dataFileName)
		{
		}
	}
}
