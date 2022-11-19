using System;

namespace PandaPlayer.LastFM.Objects
{
	public class Track
	{
		public int? Number { get; set; }

		public string Title { get; set; }

		public string Artist { get; set; }

		public Album Album { get; set; }

		public TimeSpan Duration { get; set; }

		public override string ToString()
		{
			var artist = String.IsNullOrEmpty(Artist) ? "N/A" : Artist;
			var title = String.IsNullOrEmpty(Title) ? "N/A" : Title;
			return $"{artist} - {title}";
		}
	}
}
