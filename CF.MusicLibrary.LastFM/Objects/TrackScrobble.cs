using System;

namespace CF.MusicLibrary.LastFM.Objects
{
	public class TrackScrobble
	{
		public Track Track { get; set; }

		/// <summary>
		/// The time the track started playing.
		/// </summary>
		public DateTime PlayStartTimestamp { get; set; }

		public bool ChosenByUser { get; set; }

		public TrackScrobble(Track track, DateTime playStartTimestamp)
		{
			Track = track;
			PlayStartTimestamp = playStartTimestamp;
			ChosenByUser = true;
		}
	}
}
