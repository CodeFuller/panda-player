using System;
using System.Collections.Generic;
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
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class RankBasedAdviserTests
	{
		[TestMethod]
		public async Task Advise_SortsAdviseGroups()
		{
			// Arrange

			var adviseSet1 = CreateTestAdviseSet("1");
			var adviseSet2 = CreateTestAdviseSet("2");
			var adviseGroups = CreateAdviseGroups(adviseSet1, adviseSet2).ToList();
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var adviseContentSorterStub = new Mock<IAdviseContentSorter>();
			adviseContentSorterStub.Setup(x => x.SortAdviseGroups(adviseGroups, playbacksInfo)).Returns(new[] { adviseGroups[1], adviseGroups[0] });
			adviseContentSorterStub.Setup(x => x.SortAdviseSets(It.IsAny<IEnumerable<AdviseSetContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseSetContent>, PlaybacksInfo>((adviseSets, _) => adviseSets);

			var mocker = new AutoMocker();
			mocker.Use(adviseContentSorterStub);

			var target = mocker.CreateInstance<RankBasedAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForAdviseSet(adviseSet2),
				AdvisedPlaylist.ForAdviseSet(adviseSet1),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_SortsAdviseSetsWithinAdviseGroup()
		{
			// Arrange

			var disc1 = CreateTestDisc("Disc 1");
			var disc2 = CreateTestDisc("Disc 2");

			var adviseGroup = new AdviseGroupContent("1", isFavorite: false);
			adviseGroup.AddDisc(disc1);
			adviseGroup.AddDisc(disc2);

			var adviseSets = adviseGroup.AdviseSets.ToList();

			var playbacksInfo = new PlaybacksInfo(new[] { adviseGroup });

			var adviseContentSorterStub = new Mock<IAdviseContentSorter>();
			adviseContentSorterStub.Setup(x => x.SortAdviseGroups(It.IsAny<IEnumerable<AdviseGroupContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseGroupContent>, PlaybacksInfo>((adviseGroups, _) => adviseGroups);
			adviseContentSorterStub.Setup(x => x.SortAdviseSets(adviseSets, playbacksInfo)).Returns(new[] { adviseSets[1], adviseSets[0] });

			var mocker = new AutoMocker();
			mocker.Use(adviseContentSorterStub);

			var target = mocker.CreateInstance<RankBasedAdviser>();

			// Act

			var advises = await target.Advise(new[] { adviseGroup }, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForAdviseSet(adviseSets[1]),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_IfSomeAdviseGroupsAreDeleted_SkipsDeletedAdviseGroups()
		{
			// Arrange

			var deletedAdviseSet = CreateTestAdviseSet("1", isDeleted: true);
			var activeAdviseSet = CreateTestAdviseSet("2", isDeleted: false);

			var deletedAdviseGroup = deletedAdviseSet.ToAdviseGroup();
			var activeAdviseGroup = activeAdviseSet.ToAdviseGroup();

			var adviseGroups = new[] { deletedAdviseGroup, activeAdviseGroup, };
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var adviseContentSorterStub = new Mock<IAdviseContentSorter>();
			adviseContentSorterStub.Setup(x => x.SortAdviseGroups(It.IsAny<IEnumerable<AdviseGroupContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseGroupContent>, PlaybacksInfo>((adviseGroups, _) => adviseGroups);
			adviseContentSorterStub.Setup(x => x.SortAdviseSets(It.IsAny<IEnumerable<AdviseSetContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseSetContent>, PlaybacksInfo>((adviseSets, _) => adviseSets);

			var mocker = new AutoMocker();
			mocker.Use(adviseContentSorterStub);

			var target = mocker.CreateInstance<RankBasedAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForAdviseSet(activeAdviseSet),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task Advise_IfSomeAdviseSetsAreDeleted_SkipsDeletedAdviseSets()
		{
			// Arrange

			var deletedDisc = CreateTestDisc("1", isDeleted: true);
			var activeDisc = CreateTestDisc("2", isDeleted: false);

			var adviseGroup = new AdviseGroupContent("1", isFavorite: false);
			adviseGroup.AddDisc(deletedDisc);
			adviseGroup.AddDisc(activeDisc);

			var adviseGroups = new[] { adviseGroup };
			var playbacksInfo = new PlaybacksInfo(adviseGroups);

			var adviseContentSorterStub = new Mock<IAdviseContentSorter>();
			adviseContentSorterStub.Setup(x => x.SortAdviseGroups(It.IsAny<IEnumerable<AdviseGroupContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseGroupContent>, PlaybacksInfo>((adviseGroups, _) => adviseGroups);
			adviseContentSorterStub.Setup(x => x.SortAdviseSets(It.IsAny<IEnumerable<AdviseSetContent>>(), playbacksInfo))
				.Returns<IEnumerable<AdviseSetContent>, PlaybacksInfo>((adviseSets, _) => adviseSets);

			var mocker = new AutoMocker();
			mocker.Use(adviseContentSorterStub);

			var target = mocker.CreateInstance<RankBasedAdviser>();

			// Act

			var advises = await target.Advise(adviseGroups, playbacksInfo, CancellationToken.None);

			// Assert

			var expectedAdvises = new[]
			{
				AdvisedPlaylist.ForAdviseSet(activeDisc.ToAdviseSet()),
			};

			advises.Should().BeEquivalentTo(expectedAdvises, x => x.WithStrictOrdering());
		}

		private static IReadOnlyCollection<AdviseGroupContent> CreateAdviseGroups(params AdviseSetContent[] adviseSets)
		{
			return adviseSets.Select(adviseSet => adviseSet.ToAdviseGroup()).ToList();
		}

		private static AdviseSetContent CreateTestAdviseSet(string id, bool isDeleted = false)
		{
			var disc = CreateTestDisc(id, isDeleted);
			return disc.ToAdviseSet();
		}

		private static DiscModel CreateTestDisc(string id, bool isDeleted = false)
		{
			var disc = new DiscModel
			{
				Id = new ItemId(id),
				AllSongs = new List<SongModel>
				{
					new()
					{
						Id = new ItemId(id),
						DeleteDate = isDeleted ? new DateTime(2021, 06, 27) : null,
					},
				},
			};

			var folder = new FolderModel();
			folder.AddDiscs(disc);

			return disc;
		}
	}
}
