using System;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

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

		public override string ToString()
		{
			return Current($"'{Track}' on {PlayStartTimestamp:yyyy.MM.dd HH:mm:ss}");
		}
	}
}
