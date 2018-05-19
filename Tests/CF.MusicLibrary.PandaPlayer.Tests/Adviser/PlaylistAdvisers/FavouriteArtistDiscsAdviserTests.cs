using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.Tests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser.PlaylistAdvisers
{
	[TestFixture]
	public class FavouriteArtistDiscsAdviserTests
	{
		[Test]
		public void AdviseDiscs_AdvisesOnlyDiscsOfFavouriteArtists()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists =
				{
					"Favourite Artist 1",
					"Favourite Artist 2",
				}
			};

			var favouriteArtist1 = new Artist { Name = "Favourite Artist 1" };
			var favouriteArtist2 = new Artist { Name = "Favourite Artist 2" };
			var usualArtist = new Artist { Name = "Non-favourite Artist" };

			var favouriteDisc1 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist1 } } };
			var favouriteDisc2 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist1 } } };
			var favouriteDisc3 = new Disc { SongsUnordered = new[] { new Song { Artist = favouriteArtist2 } } };

			var favouriteDiscAdvise1 = AdvisedPlaylist.ForDisc(favouriteDisc1);
			var favouriteDiscAdvise2 = AdvisedPlaylist.ForDisc(favouriteDisc2);
			var favouriteDiscAdvise3 = AdvisedPlaylist.ForDisc(favouriteDisc3);
			var usualDiscAdvise = AdvisedPlaylist.ForDisc(new Disc { SongsUnordered = new[] { new Song { Artist = usualArtist } } });

			var library = new DiscLibrary();

			IPlaylistAdviser discAdviserStub = Substitute.For<IPlaylistAdviser>();
			discAdviserStub.Advise(library).Returns(new[] { favouriteDiscAdvise1, favouriteDiscAdvise2, favouriteDiscAdvise3, usualDiscAdvise });

			var target = new FavouriteArtistDiscsAdviser(discAdviserStub, Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>(), settings.StubOptions());

			// Act

			var advisedDiscs = target.Advise(library);

			// Assert

			CollectionAssert.AreEqual(new[] { favouriteDisc1, favouriteDisc2, favouriteDisc3 }, advisedDiscs.Select(a => a.Disc));
		}

		[Test]
		public void AdviseDiscs_ConvertsFromDiscAdviseToFavouriteArtistDiscAdviseCorrectly()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = { "Favourite Artist" }
			};

			var library = new DiscLibrary();

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };
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
				FavouriteArtists =
				{
					"Favourite Artist 1",
					"Favourite Artist 2",
				}
			};

			var disc1 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist 1" } } } };
			var disc2 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist 2" } } } };
			var disc3 = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Unfavourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc1, disc2, disc3 });

			var loggerMock = Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());

			// Act

			target.Advise(library);

			// Assert

			loggerMock.DidNotReceiveWithAnyArgs().Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<FormattedLogValues, Exception, string>>());
		}

		[Test]
		public void AdviseDiscs_OnFirstCallIfSomeUnknownArtistsAreConfigured_LogsWarning()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = { "Unknown Favourite Artist" }
			};

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc });

			var loggerMock = Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());

			// Act

			target.Advise(library);

			// Assert

			loggerMock.Received(1).Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<FormattedLogValues, Exception, string>>());
		}

		[Test]
		public void AdviseDiscs_OnSubsequentCallsIfSomeUnknownArtistsAreConfigured_DoesNotLogWarning()
		{
			// Arrange

			var settings = new FavouriteArtistsAdviserSettings
			{
				FavouriteArtists = { "Unknown Favourite Artist" }
			};

			var disc = new Disc { SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Favourite Artist" } } } };
			var library = new DiscLibrary(new[] { disc });

			var loggerMock = Substitute.For<ILogger<FavouriteArtistDiscsAdviser>>();

			var target = new FavouriteArtistDiscsAdviser(Substitute.For<IPlaylistAdviser>(), loggerMock, settings.StubOptions());
			target.Advise(library);
			loggerMock.ClearReceivedCalls();

			// Act

			target.Advise(library);

			// Assert

			loggerMock.DidNotReceiveWithAnyArgs().Log(LogLevel.Warning, Arg.Any<EventId>(), Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<FormattedLogValues, Exception, string>>());
		}
	}
}
