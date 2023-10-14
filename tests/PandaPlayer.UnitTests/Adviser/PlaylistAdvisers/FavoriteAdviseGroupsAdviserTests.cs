using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Helpers;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class FavoriteAdviseGroupsAdviserTests
	{
		[TestMethod]
		public async Task Advise_IfAllAdviseSetsAreActive_AdvisesSetsInCorrectOrder()
		{
			// Arrange

			// Last playback time for each advise group:
			//
			//     adviseGroup1: 3, 2
			//     adviseGroup2: null
			//     adviseGroup3: null, 1
			//
			// Expected order (order within advise group is provided by inner adviser and is not changed):
			//
			//     adviseSet21, adviseSet11, adviseSet31
			var adviseSet11 = CreateTestAdviseSet("11", new[] { CreateTestSong(1, new DateTime(2018, 08, 17)) });
			var adviseSet12 = CreateTestAdviseSet("12", new[] { CreateTestSong(2, new DateTime(2018, 08, 18)) });
			var adviseSet21 = CreateTestAdviseSet("21", new[] { CreateTestSong(3) });
			var adviseSet31 = CreateTestAdviseSet("31", new[] { CreateTestSong(4) });
			var adviseSet32 = CreateTestAdviseSet("32", new[] { CreateTestSong(5, new DateTime(2018, 08, 19)) });

			var adviseGroups = new[]
			{
				CreateAdviseGroup("1", isFavorite: true, adviseSet11, adviseSet12),
				CreateAdviseGroup("2", isFavorite: true, adviseSet21),
				CreateAdviseGroup("3", isFavorite: true, adviseSet31, adviseSet32),
			};

			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>())).ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);

			var target = mocker.CreateInstance<FavoriteAdviseGroupsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedPlaylists = new[]
			{
				AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSet21),
				AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSet11),
				AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSet31),
			};

			advisedPlaylists.Should().BeEquivalentTo(expectedPlaylists, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_AdvisesOnlySetsFromFavoriteAdviseGroups()
		{
			// Arrange

			var adviseSetFromFavoriteAdviseGroup = CreateTestAdviseSet("1", new[] { CreateTestSong(1) });
			var adviseSetFromNonFavoriteAdviseGroup = CreateTestAdviseSet("2", new[] { CreateTestSong(2) });

			var adviseGroups = new[]
			{
				CreateAdviseGroup("1", isFavorite: false, adviseSetFromNonFavoriteAdviseGroup),
				CreateAdviseGroup("2", isFavorite: true, adviseSetFromFavoriteAdviseGroup),
			};

			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var rankBasedAdviserStub = new Mock<IPlaylistAdviser>();
			rankBasedAdviserStub.Setup(x => x.Advise(adviseGroups, playbacksInfo, It.IsAny<CancellationToken>()))
				.ReturnsAsync(CreatedAdvisedPlaylists(adviseGroups));

			var mocker = new AutoMocker();
			mocker.Use(rankBasedAdviserStub);

			var target = mocker.CreateInstance<FavoriteAdviseGroupsAdviser>();

			// Act

			var advisedPlaylists = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedPlaylists = new[]
			{
				AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup(adviseSetFromFavoriteAdviseGroup),
			};

			advisedPlaylists.Should().BeEquivalentTo(expectedPlaylists, x => x.WithStrictOrdering());
		}

		private static IReadOnlyCollection<AdvisedPlaylist> CreatedAdvisedPlaylists(IEnumerable<AdviseGroupContent> adviseGroups)
		{
			return adviseGroups
				.SelectMany(x => x.AdviseSets)
				.Select(AdvisedPlaylist.ForAdviseSet)
				.ToList();
		}

		private static AdviseSetContent CreateTestAdviseSet(string id, IEnumerable<SongModel> songs)
		{
			return new DiscModel { Id = new ItemId(id) }
				.AddSongs(songs.ToArray())
				.AddToFolder(new FolderModel())
				.ToAdviseSet();
		}

		private static SongModel CreateTestSong(int id, DateTimeOffset? lastPlaybackTime = null, DateTimeOffset? deleteDate = null)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				LastPlaybackTime = lastPlaybackTime,
				DeleteDate = deleteDate,
			};
		}

		private static AdviseGroupContent CreateAdviseGroup(string id, bool isFavorite, params AdviseSetContent[] adviseSets)
		{
			var adviseGroup = new AdviseGroupContent(id, isFavorite: isFavorite);

			foreach (var adviseSet in adviseSets)
			{
				foreach (var disc in adviseSet.Discs)
				{
					adviseGroup.AddDisc(disc);
				}
			}

			return adviseGroup;
		}
	}
}
