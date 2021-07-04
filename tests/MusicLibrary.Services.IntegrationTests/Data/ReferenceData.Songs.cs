using System;
using System.Collections.Generic;
using System.Globalization;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.IntegrationTests.Extensions;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId SongWithOptionalPropertiesFilledId => new("1");

		public static ItemId SongWithOptionalPropertiesMissingId => new("2");

		public static ItemId SongFromNullDiscId => new("3");

		public static ItemId DeletedSongId => new("4");

		public static ItemId NextSongId => new("5");

		public SongModel SongWithOptionalPropertiesFilled { get; private set; }

		public SongModel SongWithOptionalPropertiesMissing { get; private set; }

		public SongModel SongFromNullDisc { get; private set; }

		public SongModel DeletedSong { get; private set; }

		private void FillSongs(string libraryStorageRoot)
		{
			SongWithOptionalPropertiesFilled = new()
			{
				Id = SongWithOptionalPropertiesFilledId,
				Disc = NormalDisc,
				Title = "Break The Line",
				TreeTitle = "01 - Break The Line.mp3",
				TrackNumber = 1,
				Artist = Artist1,
				Genre = Genre1,
				Duration = TimeSpan.FromMilliseconds(211957),
				Rating = RatingModel.R6,
				BitRate = 320000,
				Size = 8479581,
				Checksum = 292181681,
				LastPlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 2,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/01 - Break The Line.mp3".ToContentUri(libraryStorageRoot),
			};

			SongWithOptionalPropertiesMissing = new()
			{
				Id = SongWithOptionalPropertiesMissingId,
				Disc = NormalDisc,
				Title = "Song With Null Values",
				TreeTitle = "Song With Null Values.mp3",
				Duration = TimeSpan.FromMilliseconds(186697),
				BitRate = 320000,
				Size = 7469164,
				Checksum = 2894035568,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/Song With Null Values.mp3".ToContentUri(libraryStorageRoot),
			};

			SongFromNullDisc = new()
			{
				Id = SongFromNullDiscId,
				Disc = DiscWithNullValues,
				Title = "Song From Null Disc",
				TreeTitle = "Song From Null Disc.mp3",
				Duration = TimeSpan.FromMilliseconds(186697),
				BitRate = 320000,
				Size = 7469164,
				Checksum = 2894035568,
				ContentUri = "Foreign/Guano Apes/Disc With Null Values (CD 1)/Song From Null Disc.mp3".ToContentUri(libraryStorageRoot),
			};

			DeletedSong = new()
			{
				Id = DeletedSongId,
				Disc = DeletedDisc,
				Title = "Deleted Song",
				TreeTitle = "01 - Deleted Song.mp3",
				TrackNumber = 1,
				Artist = Artist2,
				Genre = Genre2,
				Duration = TimeSpan.FromMilliseconds(486739),
				Rating = RatingModel.R4,
				LastPlaybackTime = DateTimeOffset.Parse("2021-03-28 09:33:39.2582742+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 1,
				DeleteDate = DateTimeOffset.Parse("2021-03-28 14:10:59.3191807+03:00", CultureInfo.InvariantCulture),
			};

			NormalDisc.AllSongs = new List<SongModel> { SongWithOptionalPropertiesFilled, SongWithOptionalPropertiesMissing, };
			DiscWithNullValues.AllSongs = new List<SongModel> { SongFromNullDisc, };
			DeletedDisc.AllSongs = new List<SongModel> { DeletedSong, };
		}
	}
}
