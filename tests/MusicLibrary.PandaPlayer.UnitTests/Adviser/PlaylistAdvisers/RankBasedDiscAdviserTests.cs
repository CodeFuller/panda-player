using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;

namespace MusicLibrary.PandaPlayer.UnitTests.Adviser.PlaylistAdvisers
{
	[TestClass]
	public class RankBasedDiscAdviserTests
	{
		[TestMethod]
		public void Advise_SortsDiscGroups()
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

			var discClassifierStub = new Mock<IDiscClassifier>();
			discClassifierStub.Setup(x => x.GroupLibraryDiscs(discs)).Returns(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns(new[] { discGroup2, discGroup1 });
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discClassifierStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advisedDiscs = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(2, advisedDiscs.Count);
			Assert.AreSame(disc2, advisedDiscs[0].Disc);
			Assert.AreSame(disc1, advisedDiscs[1].Disc);
		}

		[TestMethod]
		public void Advise_SortsDiscsWithinGroup()
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

			var discClassifierStub = new Mock<IDiscClassifier>();
			discClassifierStub.Setup(x => x.GroupLibraryDiscs(discs)).Returns(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(discGroup1, playbacksInfo)).Returns(new[] { disc2, disc1 });

			var mocker = new AutoMocker();
			mocker.Use(discClassifierStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advisedDiscs = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advisedDiscs.Count);
			Assert.AreSame(disc2, advisedDiscs[0].Disc);
		}

		[TestMethod]
		public void Advise_IfSomeDiscsAreDeleted_SkipsDeletedDiscs()
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

			var discClassifierStub = new Mock<IDiscClassifier>();
			discClassifierStub.Setup(x => x.GroupLibraryDiscs(discs)).Returns(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discClassifierStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advisedDiscs = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advisedDiscs.Count);
			Assert.AreSame(activeDisc, advisedDiscs[0].Disc);
		}

		[TestMethod]
		public void Advise_SomeDiscGroupsAreEmpty_SkipsSuchGroups()
		{
			// Arrange

			var disc = CreateTestDisc(21);
			var discs = new[] { disc };
			var playbacksInfo = new PlaybacksInfo(discs);

			var discGroup1 = new DiscGroup("Group 1");

			var discGroup2 = new DiscGroup("Group 2");
			discGroup2.AddDisc(disc);

			var discGroups = new[] { discGroup1, discGroup2 };

			var discClassifierStub = new Mock<IDiscClassifier>();
			discClassifierStub.Setup(x => x.GroupLibraryDiscs(discs)).Returns(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns<IEnumerable<DiscGroup>, PlaybacksInfo>((groups, _) => groups);
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discClassifierStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advisedDiscs = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(1, advisedDiscs.Count);
			Assert.AreSame(disc, advisedDiscs[0].Disc);
		}

		[TestMethod]
		public void Advise_CreatesAdvisedPlaylistOfCorrectType()
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

			var discClassifierStub = new Mock<IDiscClassifier>();
			discClassifierStub.Setup(x => x.GroupLibraryDiscs(discs)).Returns(discGroups);

			var discGroupSorterStub = new Mock<IDiscGroupSorter>();
			discGroupSorterStub.Setup(x => x.SortDiscGroups(It.IsAny<IEnumerable<DiscGroup>>(), playbacksInfo)).Returns(new[] { discGroup2, discGroup1 });
			discGroupSorterStub.Setup(x => x.SortDiscsWithinGroup(It.IsAny<DiscGroup>(), playbacksInfo)).Returns<DiscGroup, PlaybacksInfo>((g, _) => g.Discs);

			var mocker = new AutoMocker();
			mocker.Use(discClassifierStub);
			mocker.Use(discGroupSorterStub);

			var target = mocker.CreateInstance<RankBasedDiscAdviser>();

			// Act

			var advisedDiscs = target.Advise(discs, playbacksInfo).ToList();

			// Assert

			Assert.AreEqual(2, advisedDiscs.Count);
			Assert.IsTrue(advisedDiscs.All(x => x.AdvisedPlaylistType == AdvisedPlaylistType.Disc));
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
