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
	public class FolderAdviseGroupHolderTests
	{
		[TestMethod]
		public void InitialAdviseGroupName_ReturnsFolderName()
		{
			// Arrange

			var folder = new FolderModel
			{
				Name = "Folder Name",
			};

			var target = new FolderAdviseGroupHolder(folder);

			// Act

			var initialAdviseGroupName = target.InitialAdviseGroupName;

			// Assert

			initialAdviseGroupName.Should().Be("Folder Name");
		}

		[TestMethod]
		public void CurrentAdviseGroup_ReturnsFolderAdviseGroup()
		{
			// Arrange

			var adviseGroup = new AdviseGroupModel
			{
				Id = new ItemId("Advise Group Id"),
				Name = "Advise Group Name",
			};

			var folder = new FolderModel
			{
				AdviseGroup = adviseGroup,
			};

			var target = new FolderAdviseGroupHolder(folder);

			// Act

			var currentAdviseGroup = target.CurrentAdviseGroup;

			// Assert

			currentAdviseGroup.Should().Be(adviseGroup);
		}

		[TestMethod]
		public async Task AssignAdviseGroup_InvokesAssignAdviseGroupForFolder()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Id = new ItemId("Advise Group Id"),
				Name = "Advise Group Name",
			};

			var folder = new FolderModel();

			var adviseGroupServiceMock = new Mock<IAdviseGroupService>();

			var target = new FolderAdviseGroupHolder(folder);

			// Act

			await target.AssignAdviseGroup(adviseGroupServiceMock.Object, newAdviseGroup, CancellationToken.None);

			// Assert

			adviseGroupServiceMock.Verify(x => x.AssignAdviseGroup(folder, newAdviseGroup, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_InvokesRemoveAdviseGroupForFolder()
		{
			// Arrange

			var folder = new FolderModel();

			var adviseGroupServiceMock = new Mock<IAdviseGroupService>();

			var target = new FolderAdviseGroupHolder(folder);

			// Act

			await target.RemoveAdviseGroup(adviseGroupServiceMock.Object, CancellationToken.None);

			// Assert

			adviseGroupServiceMock.Verify(x => x.RemoveAdviseGroup(folder, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
