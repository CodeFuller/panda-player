using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.LastFM.Interfaces;
using PandaPlayer.LastFM.Objects;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.Scrobbling
{
	public class PersistentScrobblesProcessor : IScrobblesProcessor
	{
		private const string ScrobblesDataKey = "ScrobblesData";

		private readonly Queue<TrackScrobble> scrobblesQueue;
		private readonly ISessionDataService sessionDataService;
		private readonly ILogger<PersistentScrobblesProcessor> logger;
		private bool loaded;

		public PersistentScrobblesProcessor(Queue<TrackScrobble> scrobblesQueue, ISessionDataService sessionDataService, ILogger<PersistentScrobblesProcessor> logger)
		{
			this.scrobblesQueue = scrobblesQueue ?? throw new ArgumentNullException(nameof(scrobblesQueue));
			this.sessionDataService = sessionDataService ?? throw new ArgumentNullException(nameof(sessionDataService));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task ProcessScrobble(TrackScrobble scrobble, IScrobbler scrobbler, CancellationToken cancellationToken)
		{
			await EnsureLoaded(cancellationToken);

			scrobblesQueue.Enqueue(scrobble);
			await SaveScrobblesQueue(cancellationToken);

			while (scrobblesQueue.Any())
			{
				var currentScrobble = scrobblesQueue.Peek();

				try
				{
					await scrobbler.Scrobble(currentScrobble, cancellationToken);
				}
#pragma warning disable CA1031 // Do not catch general exception types
				catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
				{
					logger.LogWarning($"Scrobble failed: {currentScrobble}. Error: {e.Message}. Scrobbles queue size: {scrobblesQueue.Count}");
					break;
				}

				scrobblesQueue.Dequeue();
			}

			await SaveScrobblesQueue(cancellationToken);
		}

		private async Task EnsureLoaded(CancellationToken cancellationToken)
		{
			if (loaded)
			{
				return;
			}

			var scrobbles = await sessionDataService.GetData<TrackScrobble[]>(ScrobblesDataKey, cancellationToken);
			foreach (var item in scrobbles ?? Enumerable.Empty<TrackScrobble>())
			{
				scrobblesQueue.Enqueue(item);
			}

			loaded = true;
		}

		private async Task SaveScrobblesQueue(CancellationToken cancellationToken)
		{
			if (scrobblesQueue.Any())
			{
				var scrobbles = scrobblesQueue.ToArray();
				await sessionDataService.SaveData(ScrobblesDataKey, scrobbles, cancellationToken);
			}
			else
			{
				await sessionDataService.PurgeData(ScrobblesDataKey, cancellationToken);
			}
		}
	}
}
