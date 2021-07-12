using System;

namespace PandaPlayer.Core.Models
{
	public class PlaybackModel
	{
		public ItemId Id { get; set; }

		public DateTimeOffset PlaybackTime { get; set; }
	}
}
