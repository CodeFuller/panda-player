using System;
using System.Collections.Generic;

namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class SongEntity
	{
		public int Id { get; set; }

		public int DiscId { get; set; }

		public DiscEntity Disc { get; set; }

		public int? ArtistId { get; set; }

		public ArtistEntity Artist { get; set; }

		public short? TrackNumber { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		public int? GenreId { get; set; }

		public GenreEntity Genre { get; set; }

		public double DurationInMilliseconds { get; set; }

		public int? Rating { get; set; }

		public long? FileSize { get; set; }

		public int? Checksum { get; set; }

		public int? BitRate { get; set; }

		public DateTimeOffset? LastPlaybackTime { get; set; }

		public int PlaybacksCount { get; set; }

		public ICollection<PlaybackEntity> Playbacks { get; set; }

		public DateTimeOffset? DeleteDate { get; set; }
	}
}
