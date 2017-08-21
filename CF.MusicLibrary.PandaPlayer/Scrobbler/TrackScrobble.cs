using System;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public class TrackScrobble
	{
		public Track Track { get; set; }

		/// <summary>
		/// The time the track started playing.
		/// </summary>
		public DateTime Timestamp { get; set; }

		public bool ChosenByUser { get; set; }

		public TrackScrobble(Track track, DateTime timestamp)
		{
			Track = track;
			Timestamp = timestamp;
			ChosenByUser = true;
		}
	}
}
