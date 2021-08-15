using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class CompositePlaylistAdviserTests
	{
		[TestMethod]
		public async Task Advise_IfHighlyRatedSongsAdvisesProvided_AdvisesHighlyRatedSongsAtConfiguredIntervals()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings
				{
					PlaybacksBetweenHighlyRatedSongs = 10,
				},
			};

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))).ToList());

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				StubEmptyPlaylistAdviser(), Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count(x => x.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs));
			Assert.AreSame(highlyRatedSongsAdvise1, advises[0]);
			Assert.AreSame(highlyRatedSongsAdvise2, advises[10]);
		}

		[TestMethod]
		public async Task Advise_IfPlaylistAdviserMemoIsLoaded_UsesPreviousPlaybacksSinceHighlyRatedSongs()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings
				{
					PlaybacksBetweenHighlyRatedSongs = 10,
				},
			};

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))).ToList());

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var sessionDataServiceStub = new Mock<ISessionDataService>();
			sessionDataServiceStub.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 2, playbacksSinceFavoriteArtistDisc: 0));

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				StubEmptyPlaylistAdviser(), sessionDataServiceStub.Object, Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count(x => x.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs));
			Assert.AreSame(highlyRatedSongsAdvise1, advises[7]);
			Assert.AreSame(highlyRatedSongsAdvise2, advises[17]);
		}

		[TestMethod]
		public async Task Advise_IfFavoriteArtistDiscsAdvisesProvided_AdvisesFavoriteArtistDiscsAtConfiguredIntervals()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				FavoriteArtistsAdviser = new FavoriteArtistsAdviserSettings
				{
					PlaybacksBetweenFavoriteArtistDiscs = 10,
				},
			};

			var favoriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(101));
			var favoriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(102));

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))).ToList());

			var favoriteArtistDiscAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteArtistDiscAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteArtistDiscAdvise1, favoriteArtistDiscAdvise2 });

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteArtistDiscAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count(x => x.AdvisedPlaylistType == AdvisedPlaylistType.FavoriteArtistDisc));
			Assert.AreSame(favoriteArtistDiscAdvise1, advises[0]);
			Assert.AreSame(favoriteArtistDiscAdvise2, advises[10]);
		}

		[TestMethod]
		public async Task Advise_IfPlaylistAdviserMemoIsLoaded_UsesPreviousPlaybacksSinceFavoriteArtistDisc()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				FavoriteArtistsAdviser = new FavoriteArtistsAdviserSettings
				{
					PlaybacksBetweenFavoriteArtistDiscs = 10,
				},
			};

			var favoriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(101));
			var favoriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(102));

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))).ToList());

			var favoriteArtistDiscAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteArtistDiscAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteArtistDiscAdvise1, favoriteArtistDiscAdvise2 });

			var sessionDataServiceStub = new Mock<ISessionDataService>();
			sessionDataServiceStub.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 0, playbacksSinceFavoriteArtistDisc: 2));

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteArtistDiscAdviserStub.Object, sessionDataServiceStub.Object, Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count(x => x.AdvisedPlaylistType == AdvisedPlaylistType.FavoriteArtistDisc));
			Assert.AreSame(favoriteArtistDiscAdvise1, advises[7]);
			Assert.AreSame(favoriteArtistDiscAdvise2, advises[17]);
		}

		[TestMethod]
		public async Task Advise_SkipsDuplicatedDiscsBetweenDiscAdviserAndFavoriteArtistDiscs()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				FavoriteArtistsAdviser = new FavoriteArtistsAdviserSettings
				{
					PlaybacksBetweenFavoriteArtistDiscs = 10,
				},
			};

			var disc1 = CreateTestDisc(101);
			var disc2 = CreateTestDisc(102);

			var favoriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavoriteArtistDisc(disc1);
			var favoriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavoriteArtistDisc(disc2);

			var rankedDiscAdvises = Enumerable.Range(0, 10).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i)))
				.Concat(new[] { AdvisedPlaylist.ForDisc(disc1) })
				.Concat(Enumerable.Range(10, 20).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))))
				.Concat(new[] { AdvisedPlaylist.ForDisc(disc2) })
				.ToList();

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(rankedDiscAdvises);

			var favoriteArtistDiscAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteArtistDiscAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteArtistDiscAdvise1, favoriteArtistDiscAdvise2 });

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteArtistDiscAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count(a => a.Disc == disc1));
			Assert.AreEqual(1, advises.Count(a => a.Disc == disc2));
		}

		[TestMethod]
		public async Task Advise_MixesDifferentAdvisesTypesCorrectly()
		{
			// Arrange

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			var favoriteArtistDiscAdvise1 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(101));
			var favoriteArtistDiscAdvise2 = AdvisedPlaylist.ForFavoriteArtistDisc(CreateTestDisc(102));

			var rankedDiscsAdvises = Enumerable.Range(1, 10).Select(i => AdvisedPlaylist.ForDisc(CreateTestDisc(i))).ToList();

			var rankedDiscsAdviserStub = new Mock<IPlaylistAdviser>();
			rankedDiscsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(rankedDiscsAdvises);

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var favoriteArtistDiscAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteArtistDiscAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteArtistDiscAdvise1, favoriteArtistDiscAdvise2 });

			var settings = new AdviserSettings
			{
				FavoriteArtistsAdviser = new FavoriteArtistsAdviserSettings
				{
					PlaybacksBetweenFavoriteArtistDiscs = 5,
				},

				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings
				{
					PlaybacksBetweenHighlyRatedSongs = 10,
				},
			};

			var target = new CompositePlaylistAdviser(rankedDiscsAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				favoriteArtistDiscAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			var expectedAdvises = new[]
			{
						highlyRatedSongsAdvise1,
						favoriteArtistDiscAdvise1,
						rankedDiscsAdvises[0], rankedDiscsAdvises[1], rankedDiscsAdvises[2], rankedDiscsAdvises[3],
						favoriteArtistDiscAdvise2,
						rankedDiscsAdvises[4], rankedDiscsAdvises[5], rankedDiscsAdvises[6],
						highlyRatedSongsAdvise2,
						rankedDiscsAdvises[7], rankedDiscsAdvises[8], rankedDiscsAdvises[9],
			};

			CollectionAssert.AreEqual(expectedAdvises, advises);
		}

		[TestMethod]
		public async Task RegisterAdvicePlayback_SavesUpdatedPlaybacksMemoInSessionData()
		{
			// Arrange

			var sessionDataServiceMock = new Mock<ISessionDataService>();
			sessionDataServiceMock.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5));

			var target = new CompositePlaylistAdviser(StubEmptyPlaylistAdviser(), StubEmptyPlaylistAdviser(),
				StubEmptyPlaylistAdviser(), sessionDataServiceMock.Object, Options.Create(new AdviserSettings()));

			// This call is required for initializing playbacks memo.
			await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None);

			// Act

			await target.RegisterAdvicePlayback(AdvisedPlaylist.ForDisc(CreateTestDisc(1)), CancellationToken.None);

			// Assert

			Func<PlaylistAdviserMemo, bool> validateSavedMemo = memo => memo.PlaybacksSinceHighlyRatedSongsPlaylist == 4 && memo.PlaybacksSinceFavoriteArtistDisc == 6;
			sessionDataServiceMock.Verify(x => x.SaveData("PlaylistAdviserData", It.Is<PlaylistAdviserMemo>(memo => validateSavedMemo(memo)), CancellationToken.None), Times.Once);
		}

		private static IPlaylistAdviser StubEmptyPlaylistAdviser()
		{
			var playlistAdviserStub = new Mock<IPlaylistAdviser>();

			playlistAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Array.Empty<AdvisedPlaylist>());

			return playlistAdviserStub.Object;
		}

		private static DiscModel CreateTestDisc(int discId)
		{
			return new()
			{
				Id = new ItemId(discId.ToString(CultureInfo.InvariantCulture)),
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>(),
			};
		}
	}
}
