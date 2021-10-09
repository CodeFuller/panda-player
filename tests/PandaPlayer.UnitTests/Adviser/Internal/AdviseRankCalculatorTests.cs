using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;
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

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { song });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateSongRank(song, playbacksInfo);

			// Assert

			rank.Should().Be(Double.MaxValue);
		}

		[TestMethod]
		public void CalculateSongRank_ForListenedSongWithRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var song1 = CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26));
			var song2 = CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27));
			var song3 = CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28));

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { song1, song2, song3 });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateSongRank(song1, playbacksInfo);
			var rank2 = target.CalculateSongRank(song2, playbacksInfo);
			var rank3 = target.CalculateSongRank(song3, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 12)
			rank1.Should().Be(91.125);

			// (rating: 1.5 ^ 7) * (playbacks age: 11)
			rank2.Should().Be(187.9453125);

			// (rating: 1.5 ^ 4) * (playbacks age: 10)
			rank3.Should().Be(50.625);
		}

		[TestMethod]
		public void CalculateSongRank_ForListenedSongWithNoRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var song = CreateTestSong(11, rating: null, new DateTime(2021, 06, 28));

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { song });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateSongRank(song, playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 10)
			rank.Should().Be(75.9375);
		}

		[TestMethod]
		public void CalculateAdviseSetRank_ForAdviseSetWithoutPlaybacks_ReturnsMaxRank()
		{
			// Arrange

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { CreateTestSong(11, lastPlaybackTime: null) });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseSetRank(adviseGroupContent1.AdviseSets.Single(), playbacksInfo);

			// Assert

			rank.Should().Be(Double.MaxValue);
		}

		[TestMethod]
		public void CalculateAdviseSetRank_ForListenedAdviseSetWithRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) });
			var adviseGroupContent3 = CreateTestAdviseGroupContent("3", new[] { CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28)) });
			var adviseGroupContent4 = CreateTestAdviseGroupContent("4", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2, adviseGroupContent3, adviseGroupContent4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateAdviseSetRank(adviseGroupContent1.AdviseSets.Single(), playbacksInfo);
			var rank2 = target.CalculateAdviseSetRank(adviseGroupContent2.AdviseSets.Single(), playbacksInfo);
			var rank3 = target.CalculateAdviseSetRank(adviseGroupContent3.AdviseSets.Single(), playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 3)
			rank1.Should().Be(22.78125);

			// (rating: 1.5 ^ 7) * (playbacks age: 2)
			rank2.Should().Be(34.171875);

			// (rating: 1.5 ^ 4) * (playbacks age: 1)
			rank3.Should().Be(5.0625);
		}

		[TestMethod]
		public void CalculateAdviseSetRank_ForAdviseSetWithMixedRatings_ReturnsCorrectRank()
		{
			// Arrange

			var songs = new[]
			{
				CreateTestSong(11, rating: RatingModel.R2, new DateTime(2021, 06, 28)),
				CreateTestSong(12, rating: RatingModel.R3, new DateTime(2021, 06, 28)),
				CreateTestSong(13, rating: RatingModel.R5, new DateTime(2021, 06, 28)),
				CreateTestSong(14, rating: RatingModel.R4, new DateTime(2021, 06, 28)),
			};

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", songs);
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseSetRank(adviseGroupContent1.AdviseSets.Single(), playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 2.5) * (playbacks age: 1)
			rank.Should().Be(4.133513940946613);
		}

		[TestMethod]
		public void CalculateAdviseSetRank_ForListenedAdviseSetWithNoRatingDefined_ReturnsCorrectRank()
		{
			// Arrange

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { CreateTestSong(11, rating: null, new DateTime(2021, 06, 28)) });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseSetRank(adviseGroupContent1.AdviseSets.Single(), playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 5) * (playbacks age: 1)
			rank.Should().Be(7.59375);
		}

		[TestMethod]
		public void CalculateAdviseSetRank_ForAdviseSetWithMultipleDiscs_ReturnsCorrectRank()
		{
			// Arrange

			var disc1 = CreateTestDisc("1.1", new[] { CreateTestSong(1, RatingModel.R5, new DateTime(2021, 08, 28)) });
			var disc2 = CreateTestDisc("1.2", new[] { CreateTestSong(2, RatingModel.R7, new DateTime(2021, 08, 28)) });
			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			disc1.AdviseSetInfo = new AdviseSetInfo(adviseSet, 1);
			disc2.AdviseSetInfo = new AdviseSetInfo(adviseSet, 2);

			var adviseGroupContent1 = new AdviseGroupContent("1", isFavorite: false);
			adviseGroupContent1.AddDisc(disc1);
			adviseGroupContent1.AddDisc(disc2);

			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(11, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 08, 27))));
			var adviseGroupContent3 = CreateTestAdviseGroupContent("3", Enumerable.Range(21, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 08, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2, adviseGroupContent3 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseSetRank(adviseGroupContent1.AdviseSets.Single(), playbacksInfo);

			// Assert

			// (rating: 1.5 ^ 6) * (playbacks age: 1)
			rank.Should().Be(11.390625);
		}

		[TestMethod]
		public void CalculateAdviseGroupRank_ForAdviseGroupWithoutPlaybacks_ReturnsMaxRank()
		{
			// Arrange

			var adviseGroupContent1 = new AdviseGroupContent("1", isFavorite: false);
			adviseGroupContent1.AddDisc(CreateTestDisc("1.1", new[] { CreateTestSong(11, lastPlaybackTime: null) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.2", new[] { CreateTestSong(12, lastPlaybackTime: null) }));

			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseGroupRank(adviseGroupContent1, playbacksInfo);

			// Assert

			rank.Should().Be(Double.MaxValue);
		}

		[TestMethod]
		public void CalculateAdviseGroupRank_ForAdviseGroupWithPlaybacksAndRatingsDefined_ReturnsCorrectRank()
		{
			// Arrange

			var adviseGroupContent1 = CreateTestAdviseGroupContent("1", new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) });
			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) });
			var adviseGroupContent3 = CreateTestAdviseGroupContent("3", new[] { CreateTestSong(13, RatingModel.R4, new DateTime(2021, 06, 28)) });
			var adviseGroupContent4 = CreateTestAdviseGroupContent("4", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2, adviseGroupContent3, adviseGroupContent4 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank1 = target.CalculateAdviseGroupRank(adviseGroupContent1, playbacksInfo);
			var rank2 = target.CalculateAdviseGroupRank(adviseGroupContent2, playbacksInfo);
			var rank3 = target.CalculateAdviseGroupRank(adviseGroupContent3, playbacksInfo);

			// Assert

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 5) * (playbacks age: 3)
			rank1.Should().Be(22.78125);

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 7) * (playbacks age: 2)
			rank2.Should().Be(34.171875);

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 4) * (playbacks age: 1)
			rank3.Should().Be(5.0625);
		}

		[TestMethod]
		public void CalculateAdviseGroupRank_IfAdviseGroupContainsMultipleAdviseSets_ReturnsCorrectRank()
		{
			// Arrange

			var adviseGroupContent1 = new AdviseGroupContent("1", isFavorite: false);
			adviseGroupContent1.AddDisc(CreateTestDisc("1.1", new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.2", new[] { CreateTestSong(12, RatingModel.R7, new DateTime(2021, 06, 27)) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.3", new[] { CreateTestSong(13, RatingModel.R6, new DateTime(2021, 06, 28)) }));

			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseGroupRank(adviseGroupContent1, playbacksInfo);

			// Assert

			// (discs number: 3 ^ 0.5) * (rating: 1.5 ^ 6) * (playbacks age: 1)
			rank.Should().BeApproximately(19.729141, 0.000001);
		}

		[TestMethod]
		public void CalculateAdviseGroupRank_IfAdviseGroupContainsDeletedAdviseSets_SkipsDeletedAdviseSetsWhenCountingSetsNumberAndAverageRating()
		{
			// Arrange

			var adviseGroupContent1 = new AdviseGroupContent("1", isFavorite: false);
			adviseGroupContent1.AddDisc(CreateTestDisc("1.1", new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.2", new[] { CreateTestSong(12, RatingModel.R3, new DateTime(2021, 06, 27)) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.3", new[] { CreateTestSong(13, RatingModel.R6, new DateTime(2021, 06, 28), isDeleted: true) }));

			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 10).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseGroupRank(adviseGroupContent1, playbacksInfo);

			// Assert

			// (discs number: 2 ^ 0.5) * (rating: 1.5 ^ 6) * (playbacks age: 1)
			rank.Should().BeApproximately(7.15945616, 0.000001);
		}

		[TestMethod]
		public void CalculateAdviseGroupRank_IfAdviseGroupContainsDeletedAdviseSets_ConsidersDeletedAdviseSetsForPlaybacksAge()
		{
			// Arrange

			var adviseGroupContent1 = new AdviseGroupContent("1", isFavorite: false);
			adviseGroupContent1.AddDisc(CreateTestDisc("1.1", new[] { CreateTestSong(11, RatingModel.R5, new DateTime(2021, 06, 26)) }));
			adviseGroupContent1.AddDisc(CreateTestDisc("1.2", new[] { CreateTestSong(12, RatingModel.R5, new DateTime(2021, 06, 28), isDeleted: true) }));

			var adviseGroupContent2 = CreateTestAdviseGroupContent("2", Enumerable.Range(1, 5).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 27))));
			var adviseGroupContent3 = CreateTestAdviseGroupContent("3", Enumerable.Range(6, 5).Select(id => CreateTestSong(id, RatingModel.R5, new DateTime(2021, 06, 29))));

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroupContent1, adviseGroupContent2, adviseGroupContent3 });

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AdviseRankCalculator>();

			// Act

			var rank = target.CalculateAdviseGroupRank(adviseGroupContent1, playbacksInfo);

			// Assert

			// (discs number: 1 ^ 0.5) * (rating: 1.5 ^ 5) * (playbacks age: 1)
			rank.Should().Be(7.59375);
		}

		private static AdviseGroupContent CreateTestAdviseGroupContent(string id, IEnumerable<SongModel> songs)
		{
			var disc = CreateTestDisc(id, songs);

			var adviseGroupContent = new AdviseGroupContent(id, isFavorite: false);
			adviseGroupContent.AddDisc(disc);

			return adviseGroupContent;
		}

		private static DiscModel CreateTestDisc(string id, IEnumerable<SongModel> songs)
		{
			return new()
			{
				Id = new ItemId(id),
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
