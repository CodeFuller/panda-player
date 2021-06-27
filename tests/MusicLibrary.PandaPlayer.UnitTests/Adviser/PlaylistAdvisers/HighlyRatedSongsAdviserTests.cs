using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.Core.Facades;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
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

			HighlyRatedSongsAdviser Call() => new(Mock.Of<IAdviseFactorsProvider>(), Mock.Of<IClock>(), Options.Create(settings));

			// Assert

			Assert.ThrowsException<InvalidOperationException>(Call);
		}

		[TestMethod]
		public void Advise_ReturnsSongsWithHighRatingListenedEarlierThanConfiguredTerm()
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

			var songs = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, new DateTime(2017, 09, 01, 15, 48, n))).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 07));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs, advises.Single().Songs.ToList());
		}

		[TestMethod]
		public void Advise_ReturnsHighlyRatedSongsWithoutPlaybacks()
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

			var songs = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs, advises.Single().Songs.ToList());
		}

		[TestMethod]
		public void Advise_DoesNotReturnHighlyRatedSongsListenedRecently()
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

			var songs = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, new DateTime(2017, 09, 05))).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.IsFalse(advises.Any());
		}

		[TestMethod]
		public void Advise_DoesNotReturnNotHighlyRatedSongs()
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

			var songs = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R9, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.IsFalse(advises.Any());
		}

		[TestMethod]
		public void Advise_IfTooMuchSongsAreAdvised_SplitsThemIntoSmallerPlaylists()
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

			var songs1 = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var songs2 = Enumerable.Range(13, 12).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs1), CreateTestDisc(2, songs2) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs1, advises[0].Songs.ToList());
			CollectionAssert.AreEqual(songs2, advises[1].Songs.ToList());
		}

		[TestMethod]
		public void Advise_IfNumberOfAdvisedSongsIsNotEnough_ReturnsNoPlaylists()
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

			var songs = Enumerable.Range(1, 11).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.IsFalse(advises.Any());
		}

		[TestMethod]
		public void Advise_IfNumberOfSongsInLastAdviseIsNotEnough_SkipsLastAdvise()
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

			var songs1 = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var songs2 = Enumerable.Range(13, 11).Select(n => CreateTestSong(n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs1), CreateTestDisc(2, songs2) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs1, advises[0].Songs.ToList());
		}

		[TestMethod]
		public void Advise_SongsWithDifferentRatingAreAdvised_ReturnsSongsWithHigherRatingFirst()
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
					new MaxRatingTerm
					{
						Rating = RatingModel.R9,
						Days = 60,
					},
				},
			};

			var songs1 = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R9, lastPlaybackTime: null)).ToList();
			var songs2 = Enumerable.Range(1, 12).Select(n => CreateTestSong(12 + n, RatingModel.R10, lastPlaybackTime: null)).ToList();
			var discs = new[] { CreateTestDisc(1, songs1), CreateTestDisc(2, songs2) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs2, advises[0].Songs.ToList());
			CollectionAssert.AreEqual(songs1, advises[1].Songs.ToList());
		}

		[TestMethod]
		public void Advise_IfSongsAreAdvised_ReturnsSongsWithGreaterPlaybackAgeFirst()
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

			var songs1 = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, new DateTime(2017, 02, 01 + n))).ToList();
			var songs2 = Enumerable.Range(13, 12).Select(n => CreateTestSong(n, RatingModel.R10, new DateTime(2017, 01, 01 + n))).ToList();
			var discs = new[] { CreateTestDisc(1, songs1), CreateTestDisc(2, songs2) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var mocker = new AutoMocker();
			mocker.Use<IAdviseFactorsProvider>(new AdviseFactorsProvider());
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs2, advises[0].Songs.ToList());
			CollectionAssert.AreEqual(songs1, advises[1].Songs.ToList());
		}

		[TestMethod]
		public void Advise_WhenSongsAreAdvised_OrdersThemByProductOfRatingAndPlaybackAgeFactors()
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
					new MaxRatingTerm
					{
						Rating = RatingModel.R9,
						Days = 60,
					},
				},
			};

			// Rank = 2 * 0 = 0
			var song01 = CreateTestSong(1, RatingModel.R10, new DateTime(2017, 01, 12));

			// Rank = 2 * 1 = 2
			var song02 = CreateTestSong(2, RatingModel.R10, new DateTime(2017, 01, 11));

			// Rank = 2 * 2 = 4
			var song03 = CreateTestSong(3, RatingModel.R10, new DateTime(2017, 01, 10));

			// Rank = 2 * 3 = 6
			var song04 = CreateTestSong(4, RatingModel.R10, new DateTime(2017, 01, 09));

			// Rank = 2 * 4 = 8
			var song05 = CreateTestSong(5, RatingModel.R10, new DateTime(2017, 01, 08));

			// Rank = 2 * 5 = 10
			var song06 = CreateTestSong(6, RatingModel.R10, new DateTime(2017, 01, 07));

			// Rank = 1 * 6 = 6
			var song07 = CreateTestSong(7, RatingModel.R9, new DateTime(2017, 01, 06));

			// Rank = 1 * 7 = 7
			var song08 = CreateTestSong(8, RatingModel.R9, new DateTime(2017, 01, 05));

			// Rank = 1 * 8 = 8
			var song09 = CreateTestSong(9, RatingModel.R9, new DateTime(2017, 01, 04));

			// Rank = 1 * 9 = 9
			var song10 = CreateTestSong(10, RatingModel.R9, new DateTime(2017, 01, 03));

			// Rank = 1 * 10 = 10
			var song11 = CreateTestSong(11, RatingModel.R9, new DateTime(2017, 01, 02));

			// Rank = 1 * 11 = 11
			var song12 = CreateTestSong(12, RatingModel.R9, new DateTime(2017, 01, 01));

			var songs = new[] { song01, song02, song03, song04, song05, song06, song07, song08, song09, song10, song11, song12 };
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 01));

			var adviseFactorsProvider = new Mock<IAdviseFactorsProvider>();
			adviseFactorsProvider.Setup(x => x.GetFactorForRating(RatingModel.R10)).Returns(2);
			adviseFactorsProvider.Setup(x => x.GetFactorForRating(RatingModel.R9)).Returns(1);
			adviseFactorsProvider.Setup(x => x.GetFactorForPlaybackAge(It.IsAny<int>())).Returns<int>(age => age);

			var mocker = new AutoMocker();
			mocker.Use(adviseFactorsProvider);
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(new[] { song12, song06, song11, song10, song05, song09, song08, song04, song07, song03, song02, song01 }, advises.Single().Songs.ToList());
		}

		[TestMethod]
		public void Advise_CreatesAdvisedPlaylistOfCorrectType()
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

			var songs = Enumerable.Range(1, 12).Select(n => CreateTestSong(n, RatingModel.R10, new DateTime(2017, 09, 01, 15, 48, n))).ToList();
			var discs = new[] { CreateTestDisc(1, songs) };
			var playbacksInfo = new PlaybacksInfo(discs);

			var clockStub = new Mock<IClock>();
			clockStub.Setup(x => x.Now).Returns(new DateTime(2017, 10, 07));

			var mocker = new AutoMocker();
			mocker.Use(clockStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<HighlyRatedSongsAdviser>();

			// Act

			var advises = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			Assert.AreEqual(AdvisedPlaylistType.HighlyRatedSongs, advises[0].AdvisedPlaylistType);
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
