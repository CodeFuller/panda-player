using System;

namespace MusicLibrary.Logic.Models
{
	public class PlaybackModel
	{
		public int Id { get; set; }

		public DateTimeOffset PlaybackTime { get; set; }
	}
}
