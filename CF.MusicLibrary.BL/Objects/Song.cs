using System;
using System.Collections.ObjectModel;

namespace CF.MusicLibrary.BL.Objects
{
	public class Song
	{
		public int Id { get; set; }

		public Artist Artist { get; set; }

		public short OrderNumber { get; set; }

		public short? Year { get; set; }

		public string Title { get; set; }

		public Genre Genre { get; set; }

		public TimeSpan Duration { get; set; }

		public Rating? Rating { get; set; }

		public Rating SafeRating => Rating ?? Objects.Rating.R5;

		public Uri Uri { get; set; }

		public int FileSize { get; set; }

		public int? Bitrate { get; set; }

		public DateTime? LastPlaybackTime { get; set; }

		public int PlaybacksCount { get; set; }

		public Collection<Playback> Playbacks { get; } = new Collection<Playback>();
	}
}
