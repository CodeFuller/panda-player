using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class HighlyRatedSongsAdviserTests
	{
		[TestMethod]
		public void Constructor_OneAdviseSongsNumberIsNotSet_ThrowsInvalidOperationException()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings();

			// Act

			HighlyRatedSongsAdviser Call() => new(Mock.Of<IAdviseRankCalculator>(), Mock.Of<IClock>(), Options.Create(settings));

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[TestMethod]
		public async Task Advise_ReturnsSongsWithHighRatingListenedEarlierThanConfiguredTerm()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R10, new DateTime(2017, 09, 01))).ToList();
			var adviseSet = CreateTestAdviseSet("1", songs);
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 07));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(songs),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_ReturnsHighlyRatedSongsWithoutPlaybacks()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var adviseSet = CreateTestAdviseSet("1", songs);
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(songs),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_DoesNotReturnHighlyRatedSongsListenedRecently()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R10, new DateTime(2017, 09, 05))).ToList();
			var adviseSet = CreateTestAdviseSet("1", songs);
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			advises.Should().BeEmpty();
		}

		[TestMethod]
		public async Task Advise_DoesNotReturnNonHighlyRatedSongs()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R9, lastPlaybackTime: null)).ToList();
			var adviseSet = CreateTestAdviseSet("1", songs);
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			advises.Should().BeEmpty();
		}

		[TestMethod]
		public async Task Advise_IfTooMuchSongsAreAdvised_SplitsThemIntoSmallerPlaylists()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs1 = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var songs2 = Enumerable.Range(13, 12).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var adviseGroups = CreateAdviseGroups(CreateTestAdviseSet("1", songs1), CreateTestAdviseSet("2", songs2));
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(songs1),
				AdvisedPlaylist.ForHighlyRatedSongs(songs2),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_IfNumberOfAdvisedSongsIsNotEnough_ReturnsNoPlaylists()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs = Enumerable.Range(1, 11).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var adviseSet = CreateTestAdviseSet("1", songs);
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			advises.Should().BeEmpty();
		}

		[TestMethod]
		public async Task Advise_IfNumberOfSongsInLastAdviseIsNotEnough_SkipsLastAdvise()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var songs1 = Enumerable.Range(1, 12).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var songs2 = Enumerable.Range(13, 11).Select(id => CreateTestSong(id, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var adviseGroups = CreateAdviseGroups(CreateTestAdviseSet("1", songs1), CreateTestAdviseSet("2", songs2));
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(songs1),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_IfSongsAreAdvised_ReturnsSongsWithHigherRankFirst()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 3,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
				},
			};

			var song1 = CreateTestSong(1, RatingModel.R10, new DateTime(2017, 02, 01));
			var song2 = CreateTestSong(2, RatingModel.R10, new DateTime(2017, 02, 01));
			var song3 = CreateTestSong(3, RatingModel.R10, new DateTime(2017, 02, 01));
			var adviseSet = CreateTestAdviseSet("1", new[] { song1, song2, song3 });
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateSongRank(song1, playbacksInfo)).Returns(0.25);
			adviseRankCalculatorStub.Setup(x => x.CalculateSongRank(song2, playbacksInfo)).Returns(0.75);
			adviseRankCalculatorStub.Setup(x => x.CalculateSongRank(song3, playbacksInfo)).Returns(0.50);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(new[] { song2, song3, song1 }),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_SongsHaveEqualRank_ReturnsSongsWithHigherRatingFirst()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 3,
				MaxTerms = new[]
				{
					new MaxRatingTerm
					{
						Rating = RatingModel.R10,
						Days = 30,
					},
					new MaxRatingTerm
					{
						Rating = RatingModel.R9,
						Days = 60,
					},
					new MaxRatingTerm
					{
						Rating = RatingModel.R8,
						Days = 90,
					},
				},
			};

			var song1 = CreateTestSong(1, RatingModel.R8, new DateTime(2017, 02, 01));
			var song2 = CreateTestSong(2, RatingModel.R10, new DateTime(2017, 02, 01));
			var song3 = CreateTestSong(3, RatingModel.R9, new DateTime(2017, 02, 01));
			var adviseSet = CreateTestAdviseSet("1", new[] { song1, song2, song3 });
			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateSongRank(It.IsAny<SongModel>(), playbacksInfo)).Returns(0.50);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForHighlyRatedSongs(new[] { song2, song3, song1 }),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		private static IReadOnlyCollection<AdviseGroupContent> CreateAdviseGroups(params AdviseSetContent[] adviseSets)
		{
			return adviseSets.Select(adviseSet => adviseSet.ToAdviseGroup()).ToList();
		}

		private static AdviseSetContent CreateTestAdviseSet(string id, IEnumerable<SongModel> songs)
		{
			var disc = CreateTestDisc(id, songs);
			return disc.ToAdviseSet();
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

		private static SongModel CreateTestSong(int id, RatingModel rating, DateTimeOffset? lastPlaybackTime)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				TreeTitle = $"Song {id:D3}",
				Artist = null,
				Rating = rating,
				LastPlaybackTime = lastPlaybackTime,
			};
		}
	}
}
