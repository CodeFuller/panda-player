using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.LastFM.Interfaces;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM.Internal
{
	internal class LastFMScrobbler : IScrobbler
	{
		private static TimeSpan MinScrobbledTrackDuration => TimeSpan.FromSeconds(30);

		private readonly ILastFMApiClient lastFMApiClient;
		private readonly ILogger<LastFMScrobbler> logger;

		public LastFMScrobbler(ILastFMApiClient lastFMApiClient, ILogger<LastFMScrobbler> logger)
		{
			this.lastFMApiClient = lastFMApiClient ?? throw new ArgumentNullException(nameof(lastFMApiClient));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task UpdateNowPlaying(Track track)
		{
			if (!TrackCouldBeScrobbled(track))
			{
				return;
			}

			await lastFMApiClient.UpdateNowPlaying(track);
		}

		public async Task Scrobble(TrackScrobble trackScrobble)
		{
			if (!TrackCouldBeScrobbled(trackScrobble.Track))
			{
				return;
			}

			await lastFMApiClient.Scrobble(trackScrobble);
		}

		private bool TrackCouldBeScrobbled(Track track)
		{
			if (String.IsNullOrEmpty(track.Title))
			{
				logger.LogInformation("Track will not be scrobbled because it does not have a Title");
				return false;
			}

			if (String.IsNullOrEmpty(track.Artist))
			{
				logger.LogInformation("Track will not be scrobbled because it does not have an Artist");
				return false;
			}

			if (track.Duration < MinScrobbledTrackDuration)
			{
				logger.LogInformation("Track will not be scrobbled because it is too short");
				return false;
			}

			return true;
		}
	}
}
