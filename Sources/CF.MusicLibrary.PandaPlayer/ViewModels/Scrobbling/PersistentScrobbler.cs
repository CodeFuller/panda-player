using System;
using System.Threading.Tasks;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling
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

		public async Task UpdateNowPlaying(Track track)
		{
			try
			{
				await scrobbler.UpdateNowPlaying(track);
			}
			catch (Exception e)
			{
				logger.LogWarning($"Failed to update Now Playing Track for '{track}'. Error: {e.Message}");
			}
		}

		public Task Scrobble(TrackScrobble trackScrobble)
		{
			return scrobblesProcessor.ProcessScrobble(trackScrobble, scrobbler);
		}
	}
}
