using System;
using System.Collections.Generic;
using System.Globalization;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public SongModel Song1 { get; private set; }

		public SongModel Song2 { get; private set; }

		public SongModel Song3 { get; private set; }

		private void FillSongs(string libraryStorageRoot)
		{
			Song1 = new()
			{
				Id = new ItemId("1"),
				Disc = Disc1,
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
				LastPlaybackTime = DateTimeOffset.Parse("2021-03-19 13:35:02.2626013+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 9,
				ContentUri = new Uri("Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/01 - Break The Line.mp3", UriKind.Relative),
			};

			Song2 = new()
			{
				Id = new ItemId("2"),
				Disc = Disc1,
				Title = "Song With Null Values",
				TreeTitle = "02 - Song With Null Values.mp3",
				Duration = TimeSpan.FromMilliseconds(186697),
				BitRate = 320000,
				Size = 7469164,
				Checksum = 0xAC7F7A70,
				LastPlaybackTime = DateTimeOffset.Parse("2021-03-19 13:38:11.0351513+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 8,
				ContentUri = new Uri("Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/02 - Song With Null Values.mp3", UriKind.Relative),
			};

			Song3 = new()
			{
				Id = new ItemId("3"),
				Disc = Disc3,
				Title = "Deleted Song",
				TreeTitle = "01 - Deleted Song.mp3",
				TrackNumber = 1,
				Artist = Artist2,
				Genre = Genre2,
				Duration = TimeSpan.FromMilliseconds(486739),
				Rating = RatingModel.R4,
				LastPlaybackTime = DateTimeOffset.Parse("2021-03-28 09:33:39.2582742+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 2,
				DeleteDate = DateTimeOffset.Parse("2021-03-28 14:10:59.3191807+03:00", CultureInfo.InvariantCulture),
			};

			foreach (var song in new[] { Song1, Song2 })
			{
				song.ContentUri = new Uri(new Uri(libraryStorageRoot), song.ContentUri);
			}

			Disc1.AllSongs = new List<SongModel> { Song1, Song2, };
			Disc2.AllSongs = new List<SongModel> { };
			Disc3.AllSongs = new List<SongModel> { Song3, };
		}
	}
}
