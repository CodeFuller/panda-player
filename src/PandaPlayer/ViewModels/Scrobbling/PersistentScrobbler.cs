using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.LastFM.Interfaces;
using PandaPlayer.LastFM.Objects;

namespace PandaPlayer.ViewModels.Scrobbling
{
	public class PersistentScrobbler : IScrobbler
	{
		private readonly IScrobbler scrobbler;
		private readonly IScrobblesProcessor scrobblesProcessor;
		private readonly ILogger<PersistentScrobbler> logger;

		public PersistentScrobbler(IScrobbler scrobbler, IScrobblesProcessor scrobblesProcessor, ILogger<PersistentScrobbler> logger)
		{
			this.scrobbler = scrobbler ?? throw new ArgumentNullException(nameof(scrobbler));
			this.scrobblesProcessor = scrobblesProcessor ?? throw new ArgumentNullException(nameof(scrobblesProcessor));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task UpdateNowPlaying(Track track, CancellationToken cancellationToken)
		{
			try
			{
				await scrobbler.UpdateNowPlaying(track, cancellationToken);
			}
#pragma warning disable CA1031 // Do not catch general exception types
			catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
			{
				logger.LogWarning($"Failed to update Now Playing Track for '{track}'. Error: {e.Message}");
			}
		}

		public Task Scrobble(TrackScrobble trackScrobble, CancellationToken cancellationToken)
		{
			return scrobblesProcessor.ProcessScrobble(trackScrobble, scrobbler, cancellationToken);
		}
	}
}
