using System;

namespace MusicLibrary.Core.Models
{
	public class StatisticsModel
	{
		public int ArtistsNumber { get; set; }

		public int DiscArtistsNumber { get; set; }

		public int DiscsNumber { get; set; }

		public int SongsNumber { get; set; }

		public long StorageSize { get; set; }

		public TimeSpan SongsDuration { get; set; }

		public TimeSpan PlaybacksDuration { get; set; }

		public int PlaybacksNumber { get; set; }

		public int UnheardSongsNumber { get; set; }

		public int UnratedSongsNumber { get; set; }

		public int NumberOfDiscsWithoutCoverImage { get; set; }
	}
}
