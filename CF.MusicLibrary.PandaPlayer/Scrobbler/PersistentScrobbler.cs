using System;
using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public class PersistentScrobbler : IScrobbler
	{
		private readonly ILastFMApiClient lastFmApiClient;

		//private readonly string dataDirectory;

		public PersistentScrobbler(ILastFMApiClient lastFMApiClient, string dataDirectory)
		{
			if (lastFMApiClient == null)
			{
				throw new ArgumentNullException(nameof(lastFMApiClient));
			}
			if (dataDirectory == null)
			{
				throw new ArgumentNullException(nameof(dataDirectory));
			}

			this.lastFmApiClient = lastFMApiClient;
			//this.dataDirectory = dataDirectory;
		}

		public async Task UpdateNowPlaying(Track track)
		{
			await lastFmApiClient.UpdateNowPlaying(track);
		}

		public async Task Scrobble(TrackScrobble trackScrobble)
		{
			await lastFmApiClient.Scrobble(trackScrobble);
		}
	}
}
