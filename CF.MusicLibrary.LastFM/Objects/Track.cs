using System;

namespace CF.MusicLibrary.LastFM.Objects
{
	public class Track
	{
		public int? Number { get; set; }

		public string Title { get; set; }

		public string Artist { get; set; }

		public Album Album { get; set; }

		public TimeSpan Duration { get; set; }
	}
}
