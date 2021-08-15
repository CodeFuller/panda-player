using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class FavoriteArtistDiscsAdviserTests
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
		public async Task Advise_IfAllDiscsAreActive_AdvisesDiscsInCorrectOrder()
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
			//     disc21, disc11, disc31
			var disc11 = CreateTestDisc(11, new[] { CreateTestSong(1, artist1, new DateTime(2018, 08, 17)) });
			var disc12 = CreateTestDisc(12, new[] { CreateTestSong(2, artist1, new DateTime(2018, 08, 18)) });
			var disc21 = CreateTestDisc(21, new[] { CreateTestSong(3, artist2) });
			var disc31 = CreateTestDisc(31, new[] { CreateTestSong(4, artist3) });
			var disc32 = CreateTestDisc(32, new[] { CreateTestSong(5, artist3, new DateTime(2018, 08, 19)) });

			var discs = new[] { disc11, disc12, disc21, disc31, disc32 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>())).ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			CollectionAssert.AreEqual(new[] { disc21, disc11, disc31, }, advisedPlaylists.Select(a => a.Disc).ToList());
		}

		[TestMethod]
		public async Task Advise_IfSomeDiscsAreDeleted_ConsidersAlsoLastPlaybackOfDeletedDiscs()
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

			var disc11 = CreateTestDisc(11, new[] { CreateTestSong(1, artist1, lastPlaybackTime: new DateTime(2018, 09, 30), deleteDate: new DateTime(2018, 09, 30)) });
			var disc12 = CreateTestDisc(12, new[] { CreateTestSong(2, artist1, new DateTime(2018, 09, 28)) });
			var disc21 = CreateTestDisc(21, new[] { CreateTestSong(3, artist2, new DateTime(2018, 09, 29)) });

			var discs = new[] { disc11, disc12, disc21 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { disc12, disc21 }.Select(AdvisedPlaylist.ForDisc).ToList());

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			CollectionAssert.AreEqual(new[] { disc21, disc12, }, advisedPlaylists.Select(a => a.Disc).ToList());
		}

		[TestMethod]
		public async Task Advise_AdvisesOnlyDiscsOfFavoriteArtists()
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

			var favoriteDisc = CreateTestDisc(1, new[] { CreateTestSong(1, favoriteArtist) });
			var nonFavoriteDisc = CreateTestDisc(2, new[] { CreateTestSong(2, nonFavoriteArtist) });

			var discs = new[] { nonFavoriteDisc, favoriteDisc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			var advisedDiscs = advisedPlaylists.Select(x => x.Disc).ToList();
			CollectionAssert.Contains(advisedDiscs, favoriteDisc);
			CollectionAssert.DoesNotContain(advisedDiscs, nonFavoriteDisc);
		}

		[TestMethod]
		public async Task Advise_SkipsDiscsWithoutArtist()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = new[]
				{
					"Favorite Artist",
				},
			};

			var discWithoutArtist = CreateTestDisc(1, new[] { CreateTestSong(1, artist: null) });

			var discs = new[] { discWithoutArtist };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.IsFalse(advisedPlaylists.Any());
		}

		[TestMethod]
		public async Task Advise_ConvertsFromDiscAdviseToFavoriteArtistDiscAdviseCorrectly()
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
			var favoriteDisc = CreateTestDisc(1, new[] { CreateTestSong(1, favoriteArtist) });

			var discs = new[] { favoriteDisc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			var advise = advisedPlaylists.Single();
			Assert.AreEqual(AdvisedPlaylistType.FavoriteArtistDisc, advise.AdvisedPlaylistType);
			Assert.AreSame(favoriteDisc, advise.Disc);
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

			var disc1 = CreateTestDisc(1, new[] { CreateTestSong(1, favoriteArtist1) });
			var disc2 = CreateTestDisc(2, new[] { CreateTestSong(2, favoriteArtist2) });
			var disc3 = CreateTestDisc(3, new[] { CreateTestSong(3, nonFavoriteArtist) });

			var discs = new[] { disc1, disc2, disc3 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var loggerMock = new LoggerMock<FavoriteArtistDiscsAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use<ILogger<FavoriteArtistDiscsAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(0, loggerMock[LogLevel.Warning]);
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
			var disc = CreateTestDisc(1, new[] { CreateTestSong(1, someArtist) });

			var discs = new[] { disc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var loggerMock = new LoggerMock<FavoriteArtistDiscsAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use<ILogger<FavoriteArtistDiscsAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, loggerMock[LogLevel.Warning]);
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
			var disc = CreateTestDisc(1, new[] { CreateTestSong(1, someArtist) });

			var discs = new[] { disc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var loggerMock = new LoggerMock<FavoriteArtistDiscsAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use<ILogger<FavoriteArtistDiscsAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			await target.Advise(discs, playbacksInfo, CancellationToken.None);
			await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, loggerMock[LogLevel.Warning]);
		}

		[TestMethod]
		public async Task Advise_NoFavoriteArtistsAreConfigured_LogsWarning()
		{
			// Arrange

			var settings = new FavoriteArtistsAdviserSettings
			{
				FavoriteArtists = Array.Empty<string>(),
			};

			var someArtist = new ArtistModel { Id = new ItemId("1"), Name = "Some Artist" };
			var disc = CreateTestDisc(1, new[] { CreateTestSong(1, someArtist) });

			var discs = new[] { disc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discAdviserStub = new Mock<IPlaylistAdviser>();
			discAdviserStub.Setup(x => x.Advise(discs, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(discs.Select(AdvisedPlaylist.ForDisc).ToList());

			var loggerMock = new LoggerMock<FavoriteArtistDiscsAdviser>();

			var mocker = new AutoMocker();
			mocker.Use(discAdviserStub);
			mocker.Use<ILogger<FavoriteArtistDiscsAdviser>>(loggerMock);
			mocker.Use(Options.Create(settings));

			var target = mocker.CreateInstance<FavoriteArtistDiscsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, loggerMock[LogLevel.Warning]);
			Assert.IsFalse(advisedPlaylists.Any());
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
