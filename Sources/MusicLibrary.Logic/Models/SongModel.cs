﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicLibrary.Logic.Models
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

		public ItemId ArtistId => Artist?.Id;

		public ArtistModel Artist { get; set; }

		public ItemId GenreId => Genre?.Id;

		public GenreModel Genre { get; set; }

		public RatingModel Rating { get; set; }

		public int? BitRate { get; set; }

		public long? Size { get; set; }

		public uint? Checksum { get; set; }

		public DateTimeOffset? LastPlaybackTime { get; set; }

		public int PlaybacksCount { get; set; }

		public ICollection<PlaybackModel> Playbacks { get; } = new Collection<PlaybackModel>();

		public Uri ContentUri { get; set; }

		public void AddPlayback(DateTimeOffset playbackTime)
		{
			++PlaybacksCount;
			LastPlaybackTime = playbackTime;

			var playback = new PlaybackModel
			{
				PlaybackTime = playbackTime,
			};

			Playbacks.Add(playback);
		}
	}
}
