using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class AdviseGroupServiceTests : BasicServiceTests<IAdviseGroupService>
	{
		[TestMethod]
		public async Task CreateAdviseGroup_ForNonExistingAdviseGroupName_CreatesAdviseGroupSuccessfully()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Name = "Зоопарк",
			};

			var target = CreateTestTarget();

			// Act

			await target.CreateAdviseGroup(newAdviseGroup, CancellationToken.None);

			// Assert

			var expectedAdviseGroup = new AdviseGroupModel
			{
				Id = ReferenceData.NextAdviseGroupId,
				Name = "Зоопарк",
			};

			newAdviseGroup.Should().BeEquivalentTo(expectedAdviseGroup);

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
				expectedAdviseGroup,
			};

			var allAdviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			allAdviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateAdviseGroup_ForExistingAdviseGroupName_Throws()
		{
			// Arrange

			var newAdviseGroup = new AdviseGroupModel
			{
				Name = "Late Neuro Dubel",
			};

			var target = CreateTestTarget();

			// Act

			Func<Task> call = () => target.CreateAdviseGroup(newAdviseGroup, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
			};

			var allAdviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			allAdviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task GetAllAdviseGroups_ReturnsAdviseGroupsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.AdviseGroup2,
				referenceData.AdviseGroup1,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForFolderWithoutAdviseGroup_AssignsAdviseGroupCorrectly()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.SubFolderId);
			folder.AdviseGroup.Should().BeNull();

			var adviseGroup = await GetAdviseGroup(ReferenceData.AdviseGroup1Id);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.SubFolder;
			expectedFolder.AdviseGroup = referenceData.AdviseGroup1;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.SubFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForFolderWithAdviseGroup_AssignsNewAdviseGroupCorrectly()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Id.Should().Be(ReferenceData.AdviseGroup1Id);

			var adviseGroup = await GetAdviseGroup(ReferenceData.AdviseGroup2Id);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = referenceData.AdviseGroup2;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_ForFolderWithAdviseGroup_RemovesAdviseGroupCorrectly()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Should().NotBeNull();

			var target = CreateTestTarget();

			// Act

			await target.RemoveAdviseGroup(folder, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = null;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			await CheckLibraryConsistency();
		}

		private async Task<ShallowFolderModel> GetFolder(ItemId folderId)
		{
			var folderService = GetService<IFoldersService>();
			return await folderService.GetFolder(folderId, CancellationToken.None);
		}

		private async Task<AdviseGroupModel> GetAdviseGroup(ItemId adviseGroupId)
		{
			var adviseGroupService = GetService<IAdviseGroupService>();
			var allAdviseGroups = await adviseGroupService.GetAllAdviseGroups(CancellationToken.None);
			return allAdviseGroups.Single(x => x.Id == adviseGroupId);
		}
	}
}
