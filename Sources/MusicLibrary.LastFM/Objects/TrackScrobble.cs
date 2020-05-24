using System;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.LastFM.Objects
{
	public class TrackScrobble
	{
		public Track Track { get; set; }

		public DateTimeOffset PlayStartTimestamp { get; set; }

		public bool ChosenByUser { get; set; }

		public override string ToString()
		{
			return Current($"'{Track}' on {PlayStartTimestamp:yyyy.MM.dd HH:mm:ss}");
		}
	}
}
