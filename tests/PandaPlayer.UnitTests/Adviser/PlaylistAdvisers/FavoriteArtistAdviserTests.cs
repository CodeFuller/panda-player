using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Helpers;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class FavoriteArtistAdviserTests
	{
		// It's difficult to test logging calls with NSubstitute, because FormattedLogValues got internal.
		// See https://github.com/nsubstitute/NSubstitute/issues/597 for more details.
		private class LoggerMock<T> : ILogger<T>
		{
			private readonly Dictionary<LogLevel, int> callsCounters = new();

			public int this[LogLevel level] => callsCounters.TryGetValue(level, out var count) ? count : 0;

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				callsCounters[logLevel] = this[logLevel] + 1;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public IDisposable BeginScope<TState>(TState state)
			{
				return Mock.Of<IDisposable>();
			}
		}

		[TestMethod]
		public async Task Advise_IfAllAdviseSetsAreActive_AdvisesSetsInCorrectOrder()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Artist 1",
					"Artist 2",
					"Artist 3",
					"Artist 4",
				},
			};

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Artist 1" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Artist 2" };
			var artist3 = new ArtistModel { Id = new ItemId("3"), Name = "Artist 3" };

			// Artist last playback times:
			//
			//     Artist1: 3, 2
			//     Artist2: null
			//     Artist3: null, 1
			//
			// Expected order (order within artist is provided by inner adviser and is not changed):
			//
			//     adviseSet21, adviseSet11, adviseSet31
			var adviseSet11 = CreateTestAdviseSet("11", new[] { CreateTestSong(1, artist1, new DateTime(2018, 08, 17)) });
			var adviseSet12 = CreateTestAdviseSet("12", new[] { CreateTestSong(2, artist1, new DateTime(2018, 08, 18)) });
			var adviseSet21 = CreateTestAdviseSet("21", new[] { CreateTestSong(3, artist2) });
			var adviseSet31 = CreateTestAdviseSet("31", new[] { CreateTestSong(4, artist3) });
			var adviseSet32 = CreateTestAdviseSet("32", new[] { CreateTestSong(5, artist3, new DateTime(2018, 08, 19)) });

			var adviseGroups = CreateAdviseGroups(adviseSet11, adviseSet12, adviseSet21, adviseSet31, adviseSet32);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>())).ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedPlaylists = new[]
			{
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSet21),
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSet11),
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSet31),
			};

			advisedPlaylists.Should().BeEquivalentTo(expectedPlaylists, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_IfSomeAdviseSetsAreDeleted_ConsidersAlsoLastPlaybackOfDeletedAdviseSets()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Artist 1",
					"Artist 2",
				},
			};

			var artist1 = new ArtistModel { Id = new ItemId("1"), Name = "Artist 1" };
			var artist2 = new ArtistModel { Id = new ItemId("2"), Name = "Artist 2" };

			var adviseSet11 = CreateTestAdviseSet("11", new[] { CreateTestSong(1, artist1, lastPlaybackTime: new DateTime(2018, 09, 30), deleteDate: new DateTime(2018, 09, 30)) });
			var adviseSet12 = CreateTestAdviseSet("12", new[] { CreateTestSong(2, artist1, new DateTime(2018, 09, 28)) });
			var adviseSet21 = CreateTestAdviseSet("21", new[] { CreateTestSong(3, artist2, new DateTime(2018, 09, 29)) });

			var adviseGroups = CreateAdviseGroups(adviseSet11, adviseSet12, adviseSet21).ToList();
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(new[] { adviseGroups[1], adviseGroups[2] }));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedPlaylists = new[]
			{
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSet21),
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(adviseSet12),
			};

			advisedPlaylists.Should().BeEquivalentTo(expectedPlaylists, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_AdvisesOnlySetsOfFavoriteArtists()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Favorite Artist",
				},
			};

			var favoriteArtist = new ArtistModel { Id = new ItemId("1"), Name = "Favorite Artist" };
			var nonFavoriteArtist = new ArtistModel { Id = new ItemId("2"), Name = "Non-favorite Artist" };

			var favoriteArtistAdviseSet = CreateTestAdviseSet("1", new[] { CreateTestSong(1, favoriteArtist) });
			var nonFavoriteArtistAdviseSet = CreateTestAdviseSet("2", new[] { CreateTestSong(2, nonFavoriteArtist) });

			var adviseGroups = CreateAdviseGroups(nonFavoriteArtistAdviseSet, favoriteArtistAdviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedPlaylists = new[]
			{
				AdvisedPlaylist.ForFavoriteArtistAdviseSet(favoriteArtistAdviseSet),
			};

			advisedPlaylists.Should().BeEquivalentTo(expectedPlaylists, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_SkipsAdviseSetsWithoutArtist()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Favorite Artist",
				},
			};

			var adviseSetWithoutArtist = CreateTestAdviseSet("1", new[] { CreateTestSong(1, artist: null) });

			var adviseGroups = CreateAdviseGroups(adviseSetWithoutArtist);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			advisedPlaylists.Should().BeEmpty();
		}

		[TestMethod]
		public async Task Advise_OnFirstCallIfAllArtistsPresentInLibrary_DoesNotLogWarning()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Favorite Artist 1",
					"Favorite Artist 2",
				},
			};

			var favoriteArtist1 = new ArtistModel { Id = new ItemId("1"), Name = "Favorite Artist 1" };
			var favoriteArtist2 = new ArtistModel { Id = new ItemId("2"), Name = "Favorite Artist 2" };
			var nonFavoriteArtist = new ArtistModel { Id = new ItemId("3"), Name = "Non-favorite Artist" };

			var adviseSet1 = CreateTestAdviseSet("1", new[] { CreateTestSong(1, favoriteArtist1) });
			var adviseSet2 = CreateTestAdviseSet("2", new[] { CreateTestSong(2, favoriteArtist2) });
			var adviseSet3 = CreateTestAdviseSet("3", new[] { CreateTestSong(3, nonFavoriteArtist) });

			var adviseGroups = CreateAdviseGroups(adviseSet1, adviseSet2, adviseSet3);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var loggerMock = new LoggerMock<FavoriteArtistAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use<ILogger<FavoriteArtistAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			loggerMock[LogLevel.Warning].Should().Be(0);
		}

		[TestMethod]
		public async Task Advise_OnFirstCallIfSomeUnknownArtistsAreConfigured_LogsWarning()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Unknown Favorite Artist",
				},
			};

			var someArtist = new ArtistModel { Id = new ItemId("1"), Name = "Favorite Artist" };
			var adviseSet = CreateTestAdviseSet("1", new[] { CreateTestSong(1, someArtist) });

			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var loggerMock = new LoggerMock<FavoriteArtistAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use<ILogger<FavoriteArtistAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			loggerMock[LogLevel.Warning].Should().Be(1);
		}

		[TestMethod]
		public async Task Advise_OnSubsequentCallsIfSomeUnknownArtistsAreConfigured_DoesNotLogWarning()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Unknown Favorite Artist",
				},
			};

			var someArtist = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var adviseSet = CreateTestAdviseSet("1", new[] { CreateTestSong(1, someArtist) });

			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var loggerMock = new LoggerMock<FavoriteArtistAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use<ILogger<FavoriteArtistAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);
			await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			loggerMock[LogLevel.Warning].Should().Be(1);
		}

		[TestMethod]
		public async Task Advise_IfNoFavoriteArtistsAreConfigured_LogsWarning()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = Array.Empty<string>(),
			};

			var someArtist = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var adviseSet = CreateTestAdviseSet("1", new[] { CreateTestSong(1, someArtist) });

			var adviseGroups = CreateAdviseGroups(adviseSet);
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var loggerMock = new LoggerMock<FavoriteArtistAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);
			mocker.Use<ILogger<FavoriteArtistAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			loggerMock[LogLevel.Warning].Should().Be(1);
			advisedPlaylists.Should().BeEmpty();
		}

		private static IReadOnlyCollection<AdvisedPlaylist> CreatedAdvisedPlaylists(IEnumerable<AdviseGroupContent> adviseGroups)
		{
			return adviseGroups
				.SelectMany(x => x.AdviseSets)
				.Select(AdvisedPlaylist.ForAdviseSet)
				.ToList();
		}

		private static IReadOnlyCollection<AdviseGroupContent> CreateAdviseGroups(params AdviseSetContent[] adviseSets)
		{
			return adviseSets.Select(adviseSet => adviseSet.ToAdviseGroup()).ToList();
		}

		private static AdviseSetContent CreateTestAdviseSet(string id, IEnumerable<SongModel> songs)
		{
			var disc = CreateTestDisc(id, songs);

			var adviseGroupContent = new AdviseGroupContent(id);
			adviseGroupContent.AddDisc(disc);

			return adviseGroupContent.AdviseSets.Single();
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

		private static SongModel CreateTestSong(int id, ArtistModel artist, DateTimeOffset? lastPlaybackTime = null, DateTimeOffset? deleteDate = null)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Artist = artist,
				LastPlaybackTime = lastPlaybackTime,
				DeleteDate = deleteDate,
			};
		}
	}
}
