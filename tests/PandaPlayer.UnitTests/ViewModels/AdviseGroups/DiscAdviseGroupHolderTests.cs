using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.AdviseGroups;

namespace PandaPlayer.UnitTests.ViewModels.AdviseGroups
{
	[TestClass]
	public class DiscAdviseGroupHolderTests
	{
		[TestMethod]
		public void InitialAdviseGroupName_IfDiscHasAlbumTitle_ReturnsDiscAlbumTitle()
		{
			// Arrange

			var disc = new DiscModel
			{
				AlbumTitle = "Album Title",
				Title = "Disc Title",
			};

			var target = new DiscAdviseGroupHolder(disc);

			// Act

			var initialAdviseGroupName = target.InitialAdviseGroupName;

			// Assert

			initialAdviseGroupName.Should().Be("Album Title");
		}

		[TestMethod]
		public void InitialAdviseGroupName_IfDiscHasNoAlbumTitle_ReturnsDiscTitle()
		{
			// Arrange

			var disc = new DiscModel
			{
				AlbumTitle = null,
				Title = "Disc Title",
			};

			var target = new DiscAdviseGroupHolder(disc);

			// Act

			var initialAdviseGroupName = target.InitialAdviseGroupName;

			// Assert

			initialAdviseGroupName.Should().Be("Disc Title");
		}

		[TestMethod]
		public void CurrentAdviseGroup_ReturnsDiscAdviseGroup()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Id = new ItemId("Advise Group Id"),
				Name = "Advise Group Name",
			};

			var disc = new DiscModel
			{
				AdviseGroup = adviseGroup,
			};

			var target = new DiscAdviseGroupHolder(disc);

			// Act

			var currentAdviseGroup = target.CurrentAdviseGroup;

			// Assert

			currentAdviseGroup.Should().Be(adviseGroup);
		}

		[TestMethod]
		public async Task AssignAdviseGroup_InvokesAssignAdviseGroupForDisc()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Id = new ItemId("Advise Group Id"),
				Name = "Advise Group Name",
			};

			var disc = new DiscModel();

			var adviseGroupServiceMock = new Mock<IAdviseGroupService>();

			var target = new DiscAdviseGroupHolder(disc);

			// Act

			await target.AssignAdviseGroup(adviseGroupServiceMock.Object, newAdviseGroup, CancellationToken.None);

			// Assert

			adviseGroupServiceMock.Verify(x => x.AssignAdviseGroup(disc, newAdviseGroup, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_InvokesRemoveAdviseGroupForDisc()
		{
			// Arrange

			var disc = new DiscModel();

			var adviseGroupServiceMock = new Mock<IAdviseGroupService>();

			var target = new DiscAdviseGroupHolder(disc);

			// Act

			await target.RemoveAdviseGroup(adviseGroupServiceMock.Object, CancellationToken.None);

			// Assert

			adviseGroupServiceMock.Verify(x => x.RemoveAdviseGroup(disc, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
