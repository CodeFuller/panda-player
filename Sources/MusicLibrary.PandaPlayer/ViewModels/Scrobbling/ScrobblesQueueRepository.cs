using System.Collections.Generic;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Logging;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public class ScrobblesQueueRepository : JsonFileGenericRepository<Queue<TrackScrobble>>, IScrobblesQueueRepository
	{
		public ScrobblesQueueRepository(IFileSystemFacade fileSystemFacade, ILogger<ScrobblesQueueRepository> logger, string dataFileName)
			: base(fileSystemFacade, logger, dataFileName)
		{
		}
	}
}
