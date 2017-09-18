using System;
using System.Threading.Tasks;
using CF.Library.Core;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.LastFM
{
	public class LastFMScrobbler : IScrobbler
	{
		private static TimeSpan MinScrobbledTrackDuration => TimeSpan.FromSeconds(30);

		private readonly ILastFMApiClient lastFMApiClient;

		public LastFMScrobbler(ILastFMApiClient lastFMApiClient)
		{
			if (lastFMApiClient == null)
			{
				throw new ArgumentNullException(nameof(lastFMApiClient));
			}

			this.lastFMApiClient = lastFMApiClient;
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

		private static bool TrackCouldBeScrobbled(Track track)
		{
			if (String.IsNullOrEmpty(track.Title))
			{
				Application.Logger.WriteInfo("Track will not be scrobbled because it does not have a Title");
				return false;
			}

			if (String.IsNullOrEmpty(track.Artist))
			{
				Application.Logger.WriteInfo("Track will not be scrobbled because it does not have an Artist");
				return false;
			}

			if (track.Duration < MinScrobbledTrackDuration)
			{
				Application.Logger.WriteInfo("Track will not be scrobbled because it is too short");
				return false;
			}

			return true;
		}
	}
}
