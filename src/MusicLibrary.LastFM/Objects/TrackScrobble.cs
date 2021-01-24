using System;

namespace MusicLibrary.LastFM.Objects
{
	public class TrackScrobble
	{
		public Track Track { get; set; }

		public DateTimeOffset PlayStartTimestamp { get; set; }

		public bool ChosenByUser { get; set; }

		public override string ToString()
		{
			return $"'{Track}' on {PlayStartTimestamp:yyyy.MM.dd HH:mm:ss}";
		}
	}
}
