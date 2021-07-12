using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.RankBasedAdviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser.RankBasedAdviser
{
	[TestClass]
	public class RankBasedDiscGroupSorterTests
	{
		[TestMethod]
		public void SortDiscGroups_SortsGroupsByTheirRank()
		{
			// Arrange

			var discGroup1 = new DiscGroup("group1");
			var discGroup2 = new DiscGroup("group2");
			var discGroup3 = new DiscGroup("group3");

			var playbacksInfo = new PlaybacksInfo(Enumerable.Empty<DiscModel>());

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscGroupRank(It.Is<RankedDiscGroup>(rdg => rdg.DiscGroup.Id == "group1"))).Returns(0.25);
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscGroupRank(It.Is<RankedDiscGroup>(rdg => rdg.DiscGroup.Id == "group2"))).Returns(0.75);
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscGroupRank(It.Is<RankedDiscGroup>(rdg => rdg.DiscGroup.Id == "group3"))).Returns(0.50);

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);

			var target = mocker.CreateInstance<RankBasedDiscGroupSorter>();

			// Act

			var result = target.SortDiscGroups(new[] { discGroup1, discGroup2, discGroup3 }, playbacksInfo).ToList();

			// Assert

			CollectionAssert.AreEqual(new[] { "group2", "group3", "group1" }, result.Select(x => x.Id).ToList());
		}

		[TestMethod]
		public void SortDiscsWithinGroup_SortsDiscsByTheirRank()
		{
			// Arrange

			var disc1 = CreateTestDisc(1);
			var disc2 = CreateTestDisc(2);
			var disc3 = CreateTestDisc(3);

			var discGroup = new DiscGroup("test");
			discGroup.AddDisc(disc1);
			discGroup.AddDisc(disc2);
			discGroup.AddDisc(disc3);

			var playbacksInfo = new PlaybacksInfo(Enumerable.Empty<DiscModel>());

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscRank(disc1, playbacksInfo)).Returns(0.25);
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscRank(disc2, playbacksInfo)).Returns(0.75);
			adviseRankCalculatorStub.Setup(x => x.CalculateDiscRank(disc3, playbacksInfo)).Returns(0.50);

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);

			var target = mocker.CreateInstance<RankBasedDiscGroupSorter>();

			// Act

			var result = target.SortDiscsWithinGroup(discGroup, playbacksInfo).ToList();

			// Assert

			CollectionAssert.AreEqual(new[] { disc2, disc3, disc1 }, result.ToList());
		}

		[TestMethod]
		public void SortDiscsWithinGroup_IfGroupContainsDeletedDiscs_FiltersSuchDiscs()
		{
			// Arrange

			var disc1 = CreateTestDisc(1);
			var disc2 = CreateTestDisc(2, isDeleted: true);
			var disc3 = CreateTestDisc(3);

			var discGroup = new DiscGroup("test");
			discGroup.AddDisc(disc1);
			discGroup.AddDisc(disc2);
			discGroup.AddDisc(disc3);

			var playbacksInfo = new PlaybacksInfo(Enumerable.Empty<DiscModel>());

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<RankBasedDiscGroupSorter>();

			// Act

			var result = target.SortDiscsWithinGroup(discGroup, playbacksInfo).ToList();

			// Assert

			CollectionAssert.AreEqual(new[] { disc1, disc3 }, result.ToList());
		}

		private static DiscModel CreateTestDisc(int id, bool isDeleted = false)
		{
			return new()
			{
				Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
				Folder = new ShallowFolderModel(),
				AllSongs = new[]
				{
					new SongModel
					{
						Id = new ItemId(id.ToString(CultureInfo.InvariantCulture)),
						DeleteDate = isDeleted ? new DateTime(2021, 06, 29) : null,
					},
				},
			};
		}
	}
}
