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
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;

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

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))).ToList());

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				StubEmptyPlaylistAdviser(), Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			advises.Where(x => x.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs).Should().HaveCount(2);
			advises[0].Should().BeSameAs(highlyRatedSongsAdvise1);
			advises[10].Should().BeSameAs(highlyRatedSongsAdvise2);
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

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))).ToList());

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var sessionDataServiceStub = new Mock<ISessionDataService>();
			sessionDataServiceStub.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 2, playbacksSinceFavoriteAdviseGroup: 0));

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				StubEmptyPlaylistAdviser(), sessionDataServiceStub.Object, Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			advises.Where(x => x.AdvisedPlaylistType == AdvisedPlaylistType.HighlyRatedSongs).Should().HaveCount(2);
			advises[7].Should().BeSameAs(highlyRatedSongsAdvise1);
			advises[17].Should().BeSameAs(highlyRatedSongsAdvise2);
		}

		[TestMethod]
		public async Task Advise_IfAdvisesForFavoriteAdviseGroupsProvided_AdvisesAdviseSetFromFavoriteAdviseGroupsAtConfiguredIntervals()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				PlaybacksBetweenFavoriteAdviseGroups = 10,
			};

			var favoriteAdviseGroupAdvise1 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(101));
			var favoriteAdviseGroupAdvise2 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(102));

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))).ToList());

			var favoriteAdviseGroupsAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteAdviseGroupsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteAdviseGroupAdvise1, favoriteAdviseGroupAdvise2 });

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteAdviseGroupsAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			advises.Where(x => x.AdvisedPlaylistType == AdvisedPlaylistType.AdviseSetFromFavoriteAdviseGroup).Should().HaveCount(2);
			advises[0].Should().BeSameAs(favoriteAdviseGroupAdvise1);
			advises[10].Should().BeSameAs(favoriteAdviseGroupAdvise2);
		}

		[TestMethod]
		public async Task Advise_IfPlaylistAdviserMemoIsLoaded_UsesPreviousPlaybacksSinceFavoriteAdviseGroup()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				PlaybacksBetweenFavoriteAdviseGroups = 10,
			};

			var favoriteAdviseGroupAdvise1 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(101));
			var favoriteAdviseGroupAdvise2 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(102));

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Enumerable.Range(0, 100).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))).ToList());

			var favoriteAdviseGroupsAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteAdviseGroupsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteAdviseGroupAdvise1, favoriteAdviseGroupAdvise2 });

			var sessionDataServiceStub = new Mock<ISessionDataService>();
			sessionDataServiceStub.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 0, playbacksSinceFavoriteAdviseGroup: 2));

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteAdviseGroupsAdviserStub.Object, sessionDataServiceStub.Object, Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			advises.Where(x => x.AdvisedPlaylistType == AdvisedPlaylistType.AdviseSetFromFavoriteAdviseGroup).Should().HaveCount(2);
			advises[7].Should().BeSameAs(favoriteAdviseGroupAdvise1);
			advises[17].Should().BeSameAs(favoriteAdviseGroupAdvise2);
		}

		[TestMethod]
		public async Task Advise_SkipsDuplicatedAdviseSetsBetweenAdvisers()
		{
			// Arrange

			var settings = new AdviserSettings
			{
				PlaybacksBetweenFavoriteAdviseGroups = 10,
			};

			var adviseSet1 = CreateTestAdviseSet(101);
			var adviseSet2 = CreateTestAdviseSet(102);

			var favoriteAdviseGroupAdvise1 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSet1);
			var favoriteAdviseGroupAdvise2 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSet2);

			var rankedBasedAdvises = Enumerable.Range(0, 10).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i)))
				.Concat(new[] { AdvisedPlaylist.ForAdviseSet(adviseSet1) })
				.Concat(Enumerable.Range(10, 20).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))))
				.Concat(new[] { AdvisedPlaylist.ForAdviseSet(adviseSet2) })
				.ToList();

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(rankedBasedAdvises);

			var favoriteAdviseGroupsAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteAdviseGroupsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteAdviseGroupAdvise1, favoriteAdviseGroupAdvise2 });

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, StubEmptyPlaylistAdviser(),
				favoriteAdviseGroupsAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			advises.Should().ContainSingle(a => a.AdviseSet == adviseSet1);
			advises.Should().ContainSingle(a => a.AdviseSet == adviseSet2);
		}

		[TestMethod]
		public async Task Advise_MixesDifferentAdviseTypesCorrectly()
		{
			// Arrange

			var highlyRatedSongsAdvise1 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());
			var highlyRatedSongsAdvise2 = AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>());

			var favoriteAdviseGroupAdvise1 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(101));
			var favoriteAdviseGroupAdvise2 = AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(CreateTestAdviseSet(102));

			var rankedBasedAdvises = Enumerable.Range(1, 10).Select(i => AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(i))).ToList();

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(rankedBasedAdvises);

			var highlyRatedSongsAdviserStub = new Mock<IPlaylistAdviser>();
			highlyRatedSongsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { highlyRatedSongsAdvise1, highlyRatedSongsAdvise2 });

			var favoriteAdviseGroupsAdviserStub = new Mock<IPlaylistAdviser>();
			favoriteAdviseGroupsAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[] { favoriteAdviseGroupAdvise1, favoriteAdviseGroupAdvise2 });

			var settings = new AdviserSettings
			{
				PlaybacksBetweenFavoriteAdviseGroups = 5,
				HighlyRatedSongsAdviser = new HighlyRatedSongsAdviserSettings
				{
					PlaybacksBetweenHighlyRatedSongs = 10,
				},
			};

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), rankBasedAdviserStub.Object, highlyRatedSongsAdviserStub.Object,
				favoriteAdviseGroupsAdviserStub.Object, Mock.Of<ISessionDataService>(), Options.Create(settings));

			// Act

			var advises = (await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None)).ToList();

			// Assert

			var expectedAdvises = new[]
			{
						highlyRatedSongsAdvise1,
						favoriteAdviseGroupAdvise1,
						rankedBasedAdvises[0], rankedBasedAdvises[1], rankedBasedAdvises[2], rankedBasedAdvises[3],
						favoriteAdviseGroupAdvise2,
						rankedBasedAdvises[4], rankedBasedAdvises[5], rankedBasedAdvises[6],
						highlyRatedSongsAdvise2,
						rankedBasedAdvises[7], rankedBasedAdvises[8], rankedBasedAdvises[9],
			};

			advises.Should().BeEquivalentTo(expectedAdvises);
		}

		[TestMethod]
		public async Task RegisterAdvicePlayback_SavesUpdatedPlaybacksMemoInSessionData()
		{
			// Arrange

			var sessionDataServiceMock = new Mock<ISessionDataService>();
			sessionDataServiceMock.Setup(x => x.GetData<PlaylistAdviserMemo>("PlaylistAdviserData", CancellationToken.None))
				.ReturnsAsync(new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteAdviseGroup: 5));

			var target = new CompositePlaylistAdviser(StubDiscGrouper(), StubEmptyPlaylistAdviser(), StubEmptyPlaylistAdviser(),
				StubEmptyPlaylistAdviser(), sessionDataServiceMock.Object, Options.Create(new AdviserSettings()));

			// This call is required for initializing playbacks memo.
			await target.Advise(Enumerable.Empty<DiscModel>(), 30, CancellationToken.None);

			// Act

			await target.RegisterAdvicePlayback(AdvisedPlaylist.ForAdviseSet(CreateTestAdviseSet(1)), CancellationToken.None);

			// Assert

			Func<PlaylistAdviserMemo, bool> validateSavedMemo = memo => memo.PlaybacksSinceHighlyRatedSongsPlaylist == 4 && memo.PlaybacksSinceFavoriteAdviseGroup == 6;
			sessionDataServiceMock.Verify(x => x.SaveData("PlaylistAdviserData", It.Is<PlaylistAdviserMemo>(memo => validateSavedMemo(memo)), CancellationToken.None), Times.Once);
		}

		private static IPlaylistAdviser StubEmptyPlaylistAdviser()
		{
			var playlistAdviserStub = new Mock<IPlaylistAdviser>();

			playlistAdviserStub.Setup(x => x.Advise(It.IsAny<IEnumerable<AdviseGroupContent>>(), It.IsAny<PlaybacksInfo>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(Array.Empty<AdvisedPlaylist>());

			return playlistAdviserStub.Object;
		}

		private static IDiscGrouper StubDiscGrouper()
		{
			var discGrouper = new Mock<IDiscGrouper>();
			discGrouper.Setup(x => x.GroupLibraryDiscs(It.IsAny<IEnumerable<DiscModel>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<AdviseGroupContent>());

			return discGrouper.Object;
		}

		private static AdviseSetContent CreateTestAdviseSet(int id)
		{
			var stringId = id.ToString(CultureInfo.InvariantCulture);

			var disc = new DiscModel
			{
				Id = new ItemId(stringId),
				AllSongs = new List<SongModel>(),
			};

			var folder = new FolderModel();
			folder.AddDiscs(disc);

			return disc.ToAdviseSet();
		}
	}
}
