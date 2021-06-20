using System;

namespace MusicLibrary.LastFM
{
	public class LastFmClientSettings
	{
		public Uri ApiBaseUri { get; set; }

		public string ApiKey { get; set; }

		public string SharedSecret { get; set; }

		public string SessionKey { get; set; }
	}
}
