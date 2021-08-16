using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class RankBasedDiscAdviserTests
	{
		[TestMethod]
		public async Task Advise_SortsDiscGroups()
		{
			// Arrange

			var disc1 = CreateTestDisc(1);
			var disc2 = CreateTestDisc(2);
			var discs = new[] { disc1, disc2 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");
			discGroup1.AddDisc(disc1);

			var discGroup2 = new DiscGroup("Group 2");
			discGroup2.AddDisc(disc2);

			var discGroups = new[] { discGroup1, discGroup2 };

			var discGrouperStub = new Mock<IDiscGrouper>();
			discGrouperStub.Setup(x => x.GroupLibraryDiscs(discs, It.IsAny<CancellationToken>())).ReturnsAsync(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns(new[] { discGroup2, discGroup1 });
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discGrouperStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advises = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(2, advises.Count);

			var advisesList = advises.ToList();
			Assert.AreSame(disc2, advisesList[0].Disc);
			Assert.AreSame(disc1, advisesList[1].Disc);
		}

		[TestMethod]
		public async Task Advise_SortsDiscsWithinGroup()
		{
			// Arrange

			var disc1 = CreateTestDisc(11);
			var disc2 = CreateTestDisc(12);
			var discs = new[] { disc1, disc2 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");
			discGroup1.AddDisc(disc1);
			discGroup1.AddDisc(disc2);

			var discGroups = new[] { discGroup1 };

			var discGrouperStub = new Mock<IDiscGrouper>();
			discGrouperStub.Setup(x => x.GroupLibraryDiscs(discs, It.IsAny<CancellationToken>())).ReturnsAsync(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(discGroup1, playbacksInfo)).Returns(new[] { disc2, disc1 });

			var mocker = new AutoMocker();
			mocker.Use(discGrouperStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advises = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, advises.Count);
			Assert.AreSame(disc2, advises.Single().Disc);
		}

		[TestMethod]
		public async Task Advise_IfSomeDiscsAreDeleted_SkipsDeletedDiscs()
		{
			// Arrange

			var deletedDisc = CreateTestDisc(1, isDeleted: true);
			var activeDisc = CreateTestDisc(2, isDeleted: false);
			var discs = new[] { deletedDisc, activeDisc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");
			discGroup1.AddDisc(deletedDisc);

			var discGroup2 = new DiscGroup("Group 2");
			discGroup2.AddDisc(activeDisc);

			var discGroups = new[] { discGroup1, discGroup2 };

			var discGrouperStub = new Mock<IDiscGrouper>();
			discGrouperStub.Setup(x => x.GroupLibraryDiscs(discs, It.IsAny<CancellationToken>())).ReturnsAsync(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discGrouperStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advises = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, advises.Count);
			Assert.AreSame(activeDisc, advises.Single().Disc);
		}

		[TestMethod]
		public async Task Advise_SomeDiscGroupsAreEmpty_SkipsSuchGroups()
		{
			// Arrange

			var disc = CreateTestDisc(21);
			var discs = new[] { disc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");

			var discGroup2 = new DiscGroup("Group 2");
			discGroup2.AddDisc(disc);

			var discGroups = new[] { discGroup1, discGroup2 };

			var discGrouperStub = new Mock<IDiscGrouper>();
			discGrouperStub.Setup(x => x.GroupLibraryDiscs(discs, It.IsAny<CancellationToken>())).ReturnsAsync(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discGrouperStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advises = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(1, advises.Count);
			Assert.AreSame(disc, advises.Single().Disc);
		}

		[TestMethod]
		public async Task Advise_CreatesAdvisedPlaylistOfCorrectType()
		{
			// Arrange

			var disc1 = CreateTestDisc(1);
			var disc2 = CreateTestDisc(2);
			var discs = new[] { disc1, disc2 };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");
			discGroup1.AddDisc(disc1);

			var discGroup2 = new DiscGroup("Group 2");
			discGroup2.AddDisc(disc2);

			var discGroups = new[] { discGroup1, discGroup2 };

			var discGrouperStub = new Mock<IDiscGrouper>();
			discGrouperStub.Setup(x => x.GroupLibraryDiscs(discs, It.IsAny<CancellationToken>())).ReturnsAsync(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns(new[] { discGroup2, discGroup1 });
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discGrouperStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advises = await target.Advise(discs, playbacksInfo, CancellationToken.None);

			// Assert

			Assert.AreEqual(2, advises.Count);
			Assert.IsTrue(advises.All(x => x.AdvisedPlaylistType == AdvisedPlaylistType.Disc));
		}

		private static DiscModel CreateTestDisc(int id, bool isDeleted = false)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>
				{
					new()
					{
						Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
						DeleteDate = isDeleted ? new DateTime(2021, 06, 27) : null,
					},
				},
			};
		}
	}
}
