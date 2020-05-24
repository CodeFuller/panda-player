using System;

namespace MusicLibrary.Core.Models
{
	public class PlaybackModel
	{
		public int Id { get; set; }

		public DateTimeOffset PlaybackTime { get; set; }
	}
}
