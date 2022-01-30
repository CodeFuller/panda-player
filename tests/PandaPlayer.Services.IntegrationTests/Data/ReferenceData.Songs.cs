using System;
using System.Globalization;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.IntegrationTests.Extensions;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId SongWithOptionalPropertiesFilledId1 => new("1");

		public static ItemId SongWithOptionalPropertiesFilledId2 => new("2");

		public static ItemId SongWithOptionalPropertiesMissingId => new("3");

		public static ItemId DeletedSongId => new("4");

		public static ItemId NextSongId => new("5");

		public SongModel SongWithOptionalPropertiesFilled1 { get; private set; }

		public SongModel SongWithOptionalPropertiesFilled2 { get; private set; }

		public SongModel SongWithOptionalPropertiesMissing { get; private set; }

		public SongModel DeletedSong { get; private set; }

		private void FillSongs(string libraryStorageRoot, bool fillPlaybacks)
		{
			SongWithOptionalPropertiesFilled1 = new()
			{
				Id = SongWithOptionalPropertiesFilledId1,
				Title = "Про женщин",
				TreeTitle = "01 - Про женщин.mp3",
				TrackNumber = 1,
				Artist = Artist2,
				Genre = Genre1,
				Duration = TimeSpan.FromMilliseconds(10626),
				Rating = RatingModel.R6,
				BitRate = 320000,
				Size = 405582,
				Checksum = 721007018,
				LastPlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 2,
				Playbacks = fillPlaybacks ? new PlaybackModel[]
					{
						new()
						{
							Id = new ItemId("1"),
							PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:35:02.2626013+03:00", CultureInfo.InvariantCulture),
						},
						new()
						{
							Id = new ItemId("4"),
							PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:33:53.3517221+03:00", CultureInfo.InvariantCulture),
						},
					}
					: null,
				ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/01 - Про женщин.mp3".ToContentUri(libraryStorageRoot),
			};

			SongWithOptionalPropertiesFilled2 = new()
			{
				Id = SongWithOptionalPropertiesFilledId2,
				Title = "Про жизнь дяди Саши",
				TreeTitle = "02 - Про жизнь дяди Саши.mp3",
				TrackNumber = 2,
				Artist = Artist2,
				Genre = Genre1,
				Duration = TimeSpan.FromMilliseconds(10600),
				Rating = RatingModel.R6,
				BitRate = 320000,
				Size = 404555,
				Checksum = 3829155604,
				LastPlaybackTime = DateTimeOffset.Parse("2021-04-03 10:37:42.1257252+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 2,
				Playbacks = fillPlaybacks ? new PlaybackModel[]
					{
						new()
						{
							Id = new ItemId("2"),
							PlaybackTime = DateTimeOffset.Parse("2021-03-19 13:39:13.1718232+03:00", CultureInfo.InvariantCulture),
						},
						new()
						{
							Id = new ItemId("5"),
							PlaybackTime = DateTimeOffset.Parse("2021-04-03 10:37:42.1257252+03:00", CultureInfo.InvariantCulture),
						},
					}
					: null,
				ContentUri = "Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/02 - Про жизнь дяди Саши.mp3".ToContentUri(libraryStorageRoot),
			};

			SongWithOptionalPropertiesMissing = new()
			{
				Id = SongWithOptionalPropertiesMissingId,
				Title = "Song With Missing Fields",
				TreeTitle = "Song With Missing Fields.mp3",
				Duration = TimeSpan.FromMilliseconds(11618),
				BitRate = 320000,
				Size = 445175,
				Checksum = 751499818,
				Playbacks = fillPlaybacks ? Array.Empty<PlaybackModel>() : null,
				ContentUri = "Belarusian/Neuro Dubel/Disc With Missing Fields (CD 1)/Song With Missing Fields.mp3".ToContentUri(libraryStorageRoot),
			};

			DeletedSong = new()
			{
				Id = DeletedSongId,
				Title = "Deleted Song",
				TreeTitle = "01 - Deleted Song.mp3",
				TrackNumber = 1,
				Artist = Artist2,
				Genre = Genre2,
				Duration = TimeSpan.FromMilliseconds(486739),
				Rating = RatingModel.R4,
				LastPlaybackTime = DateTimeOffset.Parse("2021-03-28 09:33:39.2582742+03:00", CultureInfo.InvariantCulture),
				PlaybacksCount = 1,
				Playbacks = fillPlaybacks ? new PlaybackModel[]
					{
						new()
						{
							Id = new ItemId("3"),
							PlaybackTime = DateTimeOffset.Parse("2021-03-28 09:33:39.2582742+03:00", CultureInfo.InvariantCulture),
						},
					}
					: null,
				DeleteDate = DateTimeOffset.Parse("2021-03-28 14:10:59.3191807+03:00", CultureInfo.InvariantCulture),
				DeleteComment = "Boring",
			};

			NormalDisc.AddSong(SongWithOptionalPropertiesFilled1);
			NormalDisc.AddSong(SongWithOptionalPropertiesFilled2);
			DiscWithMissingFields.AddSong(SongWithOptionalPropertiesMissing);
			DeletedDisc.AddSong(DeletedSong);
		}
	}
}
