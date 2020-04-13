using System;

namespace MusicLibrary.LastFM
{
	public static class LastFMConstants
	{
		public static DateTime ScrobbleStartTime => new DateTime(2012, 1, 1);

		public static TimeSpan MinScrobbledTrackLength => TimeSpan.FromSeconds(30);
	}
}
