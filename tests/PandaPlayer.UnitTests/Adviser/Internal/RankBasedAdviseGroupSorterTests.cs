using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser.Internal
{
	[TestClass]
	public class RankBasedAdviseGroupSorterTests
	{
		[TestMethod]
		public void SortAdviseGroups_SortsAdviseGroupsByTheirRank()
		{
			// Arrange

			var adviseGroup1 = new AdviseGroupContent("group1");
			var adviseGroup2 = new AdviseGroupContent("group2");
			var adviseGroup3 = new AdviseGroupContent("group3");

			var playbacksInfo = new PlaybacksInfo(Enumerable.Empty<AdviseGroupContent>());

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseGroupRank(adviseGroup1, playbacksInfo)).Returns(0.25);
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseGroupRank(adviseGroup2, playbacksInfo)).Returns(0.75);
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseGroupRank(adviseGroup3, playbacksInfo)).Returns(0.50);

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);

			var target = mocker.CreateInstance<RankBasedAdviseGroupSorter>();

			// Act

			var result = target.SortAdviseGroups(new[] { adviseGroup1, adviseGroup2, adviseGroup3 }, playbacksInfo).ToList();

			// Assert

			var expectedAdviseGroups = new[]
			{
				adviseGroup2,
				adviseGroup3,
				adviseGroup1,
			};

			result.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void SortAdviseSets_SortsAdviseSetsByTheirRank()
		{
			// Arrange

			var adviseSet1 = CreateTestAdviseSet("1");
			var adviseSet2 = CreateTestAdviseSet("2");
			var adviseSet3 = CreateTestAdviseSet("3");

			var playbacksInfo = new PlaybacksInfo(Enumerable.Empty<AdviseGroupContent>());

			var adviseRankCalculatorStub = new Mock<IAdviseRankCalculator>();
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseSetRank(adviseSet1, playbacksInfo)).Returns(0.25);
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseSetRank(adviseSet2, playbacksInfo)).Returns(0.75);
			adviseRankCalculatorStub.Setup(x => x.CalculateAdviseSetRank(adviseSet3, playbacksInfo)).Returns(0.50);

			var mocker = new AutoMocker();
			mocker.Use(adviseRankCalculatorStub);

			var target = mocker.CreateInstance<RankBasedAdviseGroupSorter>();

			// Act

			var result = target.SortAdviseSets(new[] { adviseSet1, adviseSet2, adviseSet3, }, playbacksInfo).ToList();

			// Assert

			var expectedAdviseSets = new[]
			{
				adviseSet2,
				adviseSet3,
				adviseSet1,
			};

			result.Should().BeEquivalentTo(expectedAdviseSets, x => x.WithStrictOrdering());
		}

		private static AdviseSetContent CreateTestAdviseSet(string id)
		{
			var disc = CreateTestDisc(id);
			return disc.ToAdviseSet();
		}

		private static DiscModel CreateTestDisc(string id)
		{
			return new()
			{
				Id = new ItemId(id),
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>
				{
					new()
					{
						Id = new ItemId(id),
					},
				},
			};
		}
	}
}
