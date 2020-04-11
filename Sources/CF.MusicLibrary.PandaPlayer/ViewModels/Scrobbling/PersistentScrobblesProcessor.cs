using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling
{
	public class PersistentScrobblesProcessor : IScrobblesProcessor
	{
		private readonly Queue<TrackScrobble> scrobblesQueue;
		private readonly IScrobblesQueueRepository repository;
		private readonly ILogger<PersistentScrobblesProcessor> logger;
		private bool loaded;

		public PersistentScrobblesProcessor(Queue<TrackScrobble> scrobblesQueue, IScrobblesQueueRepository repository, ILogger<PersistentScrobblesProcessor> logger)
		{
			this.scrobblesQueue = scrobblesQueue ?? throw new ArgumentNullException(nameof(scrobblesQueue));
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task ProcessScrobble(TrackScrobble scrobble, IScrobbler scrobbler)
		{
			EnsureLoaded();

			scrobblesQueue.Enqueue(scrobble);

			while (scrobblesQueue.Any())
			{
				var currScrobble = scrobblesQueue.Peek();

				try
				{
					await scrobbler.Scrobble(currScrobble);
				}
#pragma warning disable CA1031 // Do not catch general exception types
				catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
				{
					logger.LogWarning($"Scrobble failed: {currScrobble}. Error: {e.Message}. Scrobbles queue size: {scrobblesQueue.Count}");
					break;
				}

				scrobblesQueue.Dequeue();
			}

			if (scrobblesQueue.Any())
			{
				repository.Save(scrobblesQueue);
			}
			else
			{
				repository.Purge();
			}
		}

		private void EnsureLoaded()
		{
			if (loaded)
			{
				return;
			}

			foreach (var item in repository.Load() ?? Enumerable.Empty<TrackScrobble>())
			{
				scrobblesQueue.Enqueue(item);
			}

			loaded = true;
		}
	}
}
