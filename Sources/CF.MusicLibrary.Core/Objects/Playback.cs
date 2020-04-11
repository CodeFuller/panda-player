using System;

namespace CF.MusicLibrary.Core.Objects
{
	public class Playback
	{
		public int Id { get; set; }

		public int SongId { get; set; }

		public Song Song { get; set; }

		public DateTime PlaybackTime { get; set; }

		public Playback()
		{
		}

		public Playback(Song song, DateTime playbackTime)
		{
			Song = song;
			PlaybackTime = playbackTime;
		}
	}
}
