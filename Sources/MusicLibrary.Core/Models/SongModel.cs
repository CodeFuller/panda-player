using System;
using System.Collections.Generic;

namespace MusicLibrary.Core.Models
{
	// TBD: Rename SongModel to Song, DiscModel to Disc, ...
	public class SongModel
	{
		public ItemId Id { get; set; }

		public string Title { get; set; }

		public string TreeTitle { get; set; }

		public short? TrackNumber { get; set; }

		public TimeSpan Duration { get; set; }

		public DiscModel Disc { get; set; }

		public ArtistModel Artist { get; set; }

		public GenreModel Genre { get; set; }

		public RatingModel Rating { get; set; }

		public int? BitRate { get; set; }

		public long? Size { get; set; }

		public uint? Checksum { get; set; }

		public DateTimeOffset? LastPlaybackTime { get; set; }

		public int PlaybacksCount { get; set; }

		public IReadOnlyCollection<PlaybackModel> Playbacks { get; set; }

		public Uri ContentUri { get; set; }

		public DateTimeOffset? DeleteDate { get; set; }

		public bool IsDeleted => DeleteDate != null;

		public void AddPlayback(DateTimeOffset playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;
		}
	}
}
