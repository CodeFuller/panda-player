using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.AdviseGroups;

namespace PandaPlayer.UnitTests.ViewModels.AdviseGroups
{
	[TestClass]
	public class AdviseGroupHelperTests
	{
		[TestMethod]
		public async Task Load_FillsAdviseGroupsCorrectly()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var adviseGroupServiceStub = new Mock<IAdviseGroupService>();
			adviseGroupServiceStub.Setup(x => x.GetAllAdviseGroups(It.IsAny<CancellationToken>())).ReturnsAsync(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupServiceStub);

			var target = mocker.CreateInstance<AdviseGroupHelper>();

			// Act

			await target.Load(CancellationToken.None);

			// Assert

			target.AdviseGroups.Should().BeEquivalentTo(adviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task CreateAdviseGroup_CreatesAdviseGroupCorrectly()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var adviseGroupServiceMock = new Mock<IAdviseGroupService>();
			adviseGroupServiceMock.Setup(x => x.GetAllAdviseGroups(It.IsAny<CancellationToken>())).ReturnsAsync(adviseGroups);

			var adviseGroupHolderMock = new Mock<BasicAdviseGroupHolder>();

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupServiceMock);

			var target = mocker.CreateInstance<AdviseGroupHelper>();

			// Act

			await target.CreateAdviseGroup(adviseGroupHolderMock.Object, "New Advise Group", CancellationToken.None);

			// Assert

			Func<AdviseGroupModel, bool> verifyAdviseGroup = ag => ag.Id == null && ag.Name == "New Advise Group";

			adviseGroupServiceMock.Verify(x => x.CreateAdviseGroup(It.Is<AdviseGroupModel>(ag => verifyAdviseGroup(ag)), It.IsAny<CancellationToken>()), Times.Once());
			adviseGroupHolderMock.Verify(x => x.AssignAdviseGroup(adviseGroupServiceMock.Object, It.Is<AdviseGroupModel>(ag => verifyAdviseGroup(ag)), It.IsAny<CancellationToken>()), Times.Once);

			target.AdviseGroups.Should().BeEquivalentTo(adviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task ReverseAdviseGroup_IfCurrentAdviseGroupIsNull_AssignsNewAdviseGroup()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var newAdviseGroup = adviseGroups.First();

			var adviseGroupServiceStub = new Mock<IAdviseGroupService>();
			adviseGroupServiceStub.Setup(x => x.GetAllAdviseGroups(It.IsAny<CancellationToken>())).ReturnsAsync(adviseGroups);

			var adviseGroupHolderMock = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderMock.Setup(x => x.CurrentAdviseGroup).Returns<AdviseGroupModel>(null);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupServiceStub);

			var target = mocker.CreateInstance<AdviseGroupHelper>();

			// Act

			await target.ReverseAdviseGroup(adviseGroupHolderMock.Object, newAdviseGroup, CancellationToken.None);

			// Assert

			adviseGroupHolderMock.Verify(x => x.AssignAdviseGroup(adviseGroupServiceStub.Object, newAdviseGroup, It.IsAny<CancellationToken>()), Times.Once());

			target.AdviseGroups.Should().BeEquivalentTo(adviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task ReverseAdviseGroup_IfAdviseGroupsDiffer_AssignsNewAdviseGroup()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var currentAdviseGroup = adviseGroups[0];
			var newAdviseGroup = adviseGroups[1];

			var adviseGroupServiceStub = new Mock<IAdviseGroupService>();
			adviseGroupServiceStub.Setup(x => x.GetAllAdviseGroups(It.IsAny<CancellationToken>())).ReturnsAsync(adviseGroups);

			var adviseGroupHolderMock = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderMock.Setup(x => x.CurrentAdviseGroup).Returns(currentAdviseGroup);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupServiceStub);

			var target = mocker.CreateInstance<AdviseGroupHelper>();

			// Act

			await target.ReverseAdviseGroup(adviseGroupHolderMock.Object, newAdviseGroup, CancellationToken.None);

			// Assert

			adviseGroupHolderMock.Verify(x => x.AssignAdviseGroup(adviseGroupServiceStub.Object, newAdviseGroup, It.IsAny<CancellationToken>()), Times.Once());

			target.AdviseGroups.Should().BeEquivalentTo(adviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task ReverseAdviseGroup_ForSameAdviseGroup_RemovesAdviseGroup()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var currentAdviseGroup = adviseGroups.First();

			var adviseGroupServiceStub = new Mock<IAdviseGroupService>();
			adviseGroupServiceStub.Setup(x => x.GetAllAdviseGroups(It.IsAny<CancellationToken>())).ReturnsAsync(adviseGroups);

			var adviseGroupHolderMock = new Mock<BasicAdviseGroupHolder>();
			adviseGroupHolderMock.Setup(x => x.CurrentAdviseGroup).Returns(currentAdviseGroup);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupServiceStub);

			var target = mocker.CreateInstance<AdviseGroupHelper>();

			// Act

			await target.ReverseAdviseGroup(adviseGroupHolderMock.Object, currentAdviseGroup, CancellationToken.None);

			// Assert

			adviseGroupHolderMock.Verify(x => x.RemoveAdviseGroup(adviseGroupServiceStub.Object, It.IsAny<CancellationToken>()), Times.Once());

			target.AdviseGroups.Should().BeEquivalentTo(adviseGroups, x => x.WithStrictOrdering());
		}
	}
}
