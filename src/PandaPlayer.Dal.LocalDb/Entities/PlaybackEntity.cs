using System;

namespace PandaPlayer.Dal.LocalDb.Entities
{
	internal class PlaybackEntity
	{
		public int Id { get; set; }

		public int SongId { get; set; }

		public SongEntity Song { get; set; }

		public DateTimeOffset PlaybackTime { get; set; }
	}
}
