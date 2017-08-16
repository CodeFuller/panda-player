using System;
using System.ComponentModel.DataAnnotations;

namespace CF.MusicLibrary.BL.Objects
{
	public class Playback
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public Song Song { get; set; }

		public DateTime PlaybackTime { get; set; }

		public Playback(Song song, DateTime playbackTime)
		{
			Song = song;
			PlaybackTime = playbackTime;
		}
	}
}
