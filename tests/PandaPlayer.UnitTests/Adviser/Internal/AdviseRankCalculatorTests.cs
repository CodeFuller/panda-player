using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.RankBasedAdviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser.Internal
{
	[TestClass]
	public class AdviseRankCalculatorTests
	{
		[TestMethod]
		public void CalculateSongRank_ForSongWithoutPlaybacks_ReturnsMaxRank()
		{
			// Arrange

			var song = CreateTestSong(11, lastPlaybackTime: null);

			var disc1 = CreateTestDisc(1, new[] { song });
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var result = target.CalculateSongRank(song, playbacksInfo);

			// Assert

			Assert.AreEqual(Double.MaxValue, result);
		}

		[TestMethod]
		public void CalculateSongRank_ForListenedSongWithRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var song1 = CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26));
			var song2 = CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27));
			var song3 = CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28));

			var disc1 = CreateTestDisc(1, new[] { song1, song2, song3 });
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateSongRank(song1, playbacksInfo);
			var rank2 = target.CalculateSongRank(song2, playbacksInfo);
			var rank3 = target.CalculateSongRank(song3, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 12)
			Assert.AreEqual(91.125, rank1);

			// (rating: 1.5 ^ 7) * (playbacks age: 11)
			Assert.AreEqual(187.9453125, rank2);

			// (rating: 1.5 ^ 4) * (playbacks age: 10)
			Assert.AreEqual(50.625, rank3);
		}

		[TestMethod]
		public void CalculateSongRank_ForListenedSongWithNoRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var song = CreateTestSong(11, rating: null, new DateTime(2021, 06, 28));

			var disc1 = CreateTestDisc(1, new[] { song });
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateSongRank(song, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 10)
			Assert.AreEqual(75.9375, rank);
		}

		[TestMethod]
		public void CalculateDiscRank_ForDiscWithoutPlaybacks_ReturnsMaxRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, lastPlaybackTime: null) });
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var result = target.CalculateDiscRank(disc1, playbacksInfo);

			// Assert

			Assert.AreEqual(Double.MaxValue, result);
		}

		[TestMethod]
		public void CalculateDiscRank_ForListenedDiscWithRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) });
			var disc3 = CreateTestDisc(3, new[] { CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28)) });
			var disc4 = CreateTestDisc(4, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3, disc4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateDiscRank(disc1, playbacksInfo);
			var rank2 = target.CalculateDiscRank(disc2, playbacksInfo);
			var rank3 = target.CalculateDiscRank(disc3, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 3)
			Assert.AreEqual(22.78125, rank1);

			// (rating: 1.5 ^ 7) * (playbacks age: 2)
			Assert.AreEqual(34.171875, rank2);

			// (rating: 1.5 ^ 4) * (playbacks age: 1)
			Assert.AreEqual(5.0625, rank3);
		}

		[TestMethod]
		public void CalculateDiscRank_ForDiscWithMixedSongRatings_ReturnsCorrectRank()
		{
			// Arrange

			var songs = new[]
			{
				CreateTestSong(11, rating: RatingModel.R2, new DateTime(2021, 06, 28)),
				CreateTestSong(12, rating: RatingModel.R3, new DateTime(2021, 06, 28)),
				CreateTestSong(13, rating: RatingModel.R5, new DateTime(2021, 06, 28)),
				CreateTestSong(14, rating: RatingModel.R4, new DateTime(2021, 06, 28)),
			};

			var disc1 = CreateTestDisc(1, songs);
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateDiscRank(disc1, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 2.5) * (playbacks age: 1)
			Assert.AreEqual(4.133513940946613, rank);
		}

		[TestMethod]
		public void CalculateDiscRank_ForListenedDiscWithNoRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, rating: null, new DateTime(2021, 06, 28)) });
			var disc2 = CreateTestDisc(2, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateDiscRank(disc1, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 1)
			Assert.AreEqual(7.59375, rank);
		}

		[TestMethod]
		public void CalculateDiscGroupRank_ForDiscGroupWithoutPlaybacks_ReturnsMaxRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, lastPlaybackTime: null) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, lastPlaybackTime: null) });
			var disc3 = CreateTestDisc(3, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var discGroup = CreateTestDiscGroup("test", disc1, disc2);

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3 });
			var rankedDiscGroup = new RankedDiscGroup(discGroup, playbacksInfo);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var result = target.CalculateDiscGroupRank(rankedDiscGroup);

			// Assert

			Assert.AreEqual(Double.MaxValue, result);
		}

		[TestMethod]
		public void CalculateDiscGroupRank_ForDiscGroupWithPlaybacksAndRatingsDefined_ReturnsCorrectRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) });
			var disc3 = CreateTestDisc(3, new[] { CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28)) });
			var disc4 = CreateTestDisc(4, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var discGroup1 = CreateTestDiscGroup("group1", disc1);
			var discGroup2 = CreateTestDiscGroup("group2", disc2);
			var discGroup3 = CreateTestDiscGroup("group3", disc3);

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3, disc4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup1, playbacksInfo));
			var rank2 = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup2, playbacksInfo));
			var rank3 = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup3, playbacksInfo));

			// Assert

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 5) * (playbacks age: 3)
			Assert.AreEqual(22.78125, rank1);

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 7) * (playbacks age: 2)
			Assert.AreEqual(34.171875, rank2);

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 4) * (playbacks age: 1)
			Assert.AreEqual(5.0625, rank3);
		}

		[TestMethod]
		public void CalculateDiscGroupRank_DiscGroupContainsMultipleDiscs_ReturnsCorrectRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) });
			var disc3 = CreateTestDisc(3, new[] { CreateTestSong(13, RatingModel.R6, new DateTime(2021, 06, 28)) });
			var disc4 = CreateTestDisc(4, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var discGroup = CreateTestDiscGroup("group1", disc1, disc2, disc3);

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3, disc4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup, playbacksInfo));

			// Assert

			// (discs number: 3 ^ 0.5) * (rating: 1.5 ^ 6) * (playbacks age: 1)
			Assert.AreEqual(19.729141, rank, 0.000001);
		}

		[TestMethod]
		public void CalculateDiscGroupRank_DiscGroupContainsDeletedDiscs_SkipsDeletedDiscsWhenCountingDiscsNumberAndAverageRating()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, RatingModel.R3, new DateTime(2021, 06, 27)) });
			var disc3 = CreateTestDisc(3, new[] { CreateTestSong(13, RatingModel.R6, new DateTime(2021, 06, 28), isDeleted: true) });
			var disc4 = CreateTestDisc(4, Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var discGroup = CreateTestDiscGroup("group1", disc1, disc2, disc3);

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3, disc4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup, playbacksInfo));

			// Assert

			// (discs number: 2 ^ 0.5) * (rating: 1.5 ^ 6) * (playbacks age: 1)
			Assert.AreEqual(7.15945616, rank, 0.000001);
		}

		[TestMethod]
		public void CalculateDiscGroupRank_DiscGroupContainsDeletedDiscs_ConsidersDeletedDiscsWhenCounting()
		{
			// Arrange

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(12, RatingModel.R5, new DateTime(2021, 06, 28), isDeleted: true) });
			var disc3 = CreateTestDisc(3, Enumerable.Range(1, 5).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 27))));
			var disc4 = CreateTestDisc(4, Enumerable.Range(6, 5).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var discGroup = CreateTestDiscGroup("group1", disc1, disc2);

			var playbacksInfo = new PlaybacksInfo(new[] { disc1, disc2, disc3, disc4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateDiscGroupRank(new RankedDiscGroup(discGroup, playbacksInfo));

			// Assert

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 5) * (playbacks age: 1)
			Assert.AreEqual(7.59375, rank);
		}

		private static DiscGroup CreateTestDiscGroup(string id, params DiscModel[] discs)
		{
			var discGroup = new DiscGroup(id);
			foreach (var disc in discs)
			{
				discGroup.AddDisc(disc);
			}

			return discGroup;
		}

		private static DiscModel CreateTestDisc(int id, IEnumerable<SongModel> songs)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Folder = new ShallowFolderModel(),
				AllSongs = songs.ToList(),
			};
		}

		private static SongModel CreateTestSong(int id, RatingModel? rating = null, DateTimeOffset? lastPlaybackTime = null, bool isDeleted = false)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Rating = rating,
				LastPlaybackTime = lastPlaybackTime,
				DeleteDate = isDeleted ? new DateTime(2021, 06, 29) : null,
			};
		}
	}
}
