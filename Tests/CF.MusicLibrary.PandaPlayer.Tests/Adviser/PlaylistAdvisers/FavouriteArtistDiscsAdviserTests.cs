using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.Tests;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser.PlaylistAdvisers
{
	[TestFixture]
	public class FavouriteArtistDiscsAdviserTests
	{
		// It's difficult to test logging calls with NSubstitute, because FormattedLogValues got internal.
		// See https://github.com/nsubstitute/NSubstitute/issues/597 for more details.
		private class LoggerMock<T> : ILogger<T>
		{
			private readonly Dictionary<LogLevel, int> callsCounters = new Dictionary<LogLevel, int>();

			public int this[LogLevel level]
			{
				get => callsCounters.TryGetValue(level, out var count) ? count : 0;
				set => callsCounters[level] = value;
			}

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
				return Substitute.For<IDisposable>();
			}
		}

		[Test]
		public void AdviseDiscs_IfAllDiscsAreActive_AdvisesDiscsInCorrectOrder()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Artist 1",
					"Artist 2",
					"Artist 3",
					"Artist 4",
				},
			};

			var artist1 = new Artist { Name = "Artist 1" };
			var artist2 = new Artist { Name = "Artist 2" };
			var artist3 = new Artist { Name = "Artist 3" };

			// Artist last playback times:
			//
			//     Artist1: 3, 2
			//     Artist2: null
			//     Artist3: null, 1
			//
			// Expected order (order within artist is provided by inner adviser and is not changed):
			//
			//     disc21, disc11, disc31
			var disc11 = new Disc { SongsUnordered = new[] { new Song { Artist = artist1, LastPlaybackTime = new DateTime(2018, 08, 17) } } };
			var disc12 = new Disc { SongsUnordered = new[] { new Song { Artist = artist1, LastPlaybackTime = new DateTime(2018, 08, 18) } } };
			var disc21 = new Disc { SongsUnordered = new[] { new Song { Artist = artist2, LastPlaybackTime = null } } };
			var disc31 = new Disc { SongsUnordered = new[] { new Song { Artist = artist3, LastPlaybackTime = null } } };
			var disc32 = new Disc { SongsUnordered = new[] { new Song { Artist = artist3, LastPlaybackTime = new DateTime(2018, 08, 19) } } };

			var discs = new[] { disc11, disc12, disc21, disc31, disc32 };

			var library = new DiscLibrary(discs);

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(discs.Select(AdvisedPlaylist.ForDisc));

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedPlaylists = target.Advise(library);

			// Assert

			CollectionAssert.AreEqual(new[] { disc21, disc11, disc31, }, advisedPlaylists.Select(a => a.Disc));
		}

		[Test]
		public void AdviseDiscs_IfSomeDiscsAreDeleted_ConsidersLastPlaybackOfDeletedDiscs()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Artist 1",
					"Artist 2",
				},
			};

			var artist1 = new Artist { Name = "Artist 1" };
			var artist2 = new Artist { Name = "Artist 2" };

			var disc11 = new Disc { SongsUnordered = new[] { new Song { Artist = artist1, LastPlaybackTime = new DateTime(2018, 09, 30), DeleteDate = new DateTime(2018, 09, 30) } }, };
			var disc12 = new Disc { SongsUnordered = new[] { new Song { Artist = artist1, LastPlaybackTime = new DateTime(2018, 09, 28) } } };
			var disc21 = new Disc { SongsUnordered = new[] { new Song { Artist = artist2, LastPlaybackTime = new DateTime(2018, 09, 29) } } };

			var discs = new[] { disc11, disc12, disc21 };

			var library = new DiscLibrary(discs);

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(discs.Where(d => !d.IsDeleted).Select(AdvisedPlaylist.ForDisc));

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedPlaylists = target.Advise(library);

			// Assert

			CollectionAssert.AreEqual(new[] { disc21, disc12, }, advisedPlaylists.Select(a => a.Disc));
		}

		[Test]
		public void AdviseDiscs_AdvisesOnlyDiscsOfFavouriteArtists()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Favourite Artist",
				},
			};

			var favouriteArtist = new Artist { Name = "Favourite Artist" };
			var nonFavouriteArtist = new Artist { Name = "Non-favourite Artist" };

			var favouriteDisc = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist } } };
			var nonFavouriteDisc = new Disc { SongsUnordered = new[] { new Song { Artist = nonFavouriteArtist } } };

			var favouriteDiscAdvise = AdvisedPlaylist.ForDisc(favouriteDisc);
			var nonFavouriteDiscAdvise = AdvisedPlaylist.ForDisc(nonFavouriteDisc);

			var library = new DiscLibrary(new[] { favouriteDisc, nonFavouriteDisc });

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { favouriteDiscAdvise, nonFavouriteDiscAdvise });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedPlaylists = target.Advise(library);

			// Assert

			var advisedDiscs = advisedPlaylists.Select(x => x.Disc).ToList();
			CollectionAssert.Contains(advisedDiscs, favouriteDisc);
			CollectionAssert.DoesNotContain(advisedDiscs, nonFavouriteDisc);
		}

		[Test]
		public void AdviseDiscs_SkipsDiscsWithoutArtist()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Favourite Artist",
				},
			};

			var discWithoutArtist = new Disc { SongsUnordered = new[] { new Song { Artist = null } } };

			var library = new DiscLibrary(new[] { discWithoutArtist, });

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { AdvisedPlaylist.ForDisc(discWithoutArtist) });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedPlaylists = target.Advise(library);

			// Assert

			CollectionAssert.IsEmpty(advisedPlaylists);
		}

		[Test]
		public void AdviseDiscs_ConvertsFromDiscAdviseToFavouriteArtistDiscAdviseCorrectly()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Favourite Artist",
				},
			};

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };

			var library = new DiscLibrary(new[] { disc });

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { AdvisedPlaylist.ForDisc(disc) });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedDiscs = target.Advise(library);

			// Assert

			var advise = advisedDiscs.Single();
			Assert.AreEqual(AdvisedPlaylistType.FavouriteArtistDisc, advise.AdvisedPlaylistType);
			Assert.AreSame(disc, advise.Disc);
		}

		[Test]
		public void AdviseDiscs_OnFirstCallIfAllArtistsPresentInLibrary_DoesNotLogWarning()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Favourite Artist 1",
					"Favourite Artist 2",
				},
			};

			var disc1 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist 1" } } } };
			var disc2 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist 2" } } } };
			var disc3 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Unfavourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc1, disc2, disc3 });

			var loggerMock = new LoggerMock<FavouriteArtistDiscsAdviser>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());

			// Act

			target.Advise(library);

			// Assert

			Assert.AreEqual(0, loggerMock[LogLevel.Warning]);
		}

		[Test]
		public void AdviseDiscs_OnFirstCallIfSomeUnknownArtistsAreConfigured_LogsWarning()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Unknown Favourite Artist",
				},
			};

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc });

			var loggerMock = new LoggerMock<FavouriteArtistDiscsAdviser>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());

			// Act

			target.Advise(library);

			// Assert

			Assert.AreEqual(1, loggerMock[LogLevel.Warning]);
		}

		[Test]
		public void AdviseDiscs_OnSubsequentCallsIfSomeUnknownArtistsAreConfigured_DoesNotLogWarning()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = new[]
				{
					"Unknown Favourite Artist",
				},
			};

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc });

			var loggerMock = new LoggerMock<FavouriteArtistDiscsAdviser>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());
			target.Advise(library);
			loggerMock[LogLevel.Warning] = 0;

			// Act

			target.Advise(library);

			// Assert

			Assert.AreEqual(0, loggerMock[LogLevel.Warning]);
		}
	}
}
