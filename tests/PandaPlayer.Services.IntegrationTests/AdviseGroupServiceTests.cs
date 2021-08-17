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
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
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

			var referenceData = GetReferenceData();

			var newAdviseGroup = new AdviseGroupModel
			{
				Name = referenceData.FolderAdviseGroup.Name,
			};

			var target = CreateTestTarget();

			// Act

			Func<Task> call = () => target.CreateAdviseGroup(newAdviseGroup, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
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
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForFolderWithoutAdviseGroup_AssignsAdviseGroupCorrectly()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.SubFolderId);
			folder.AdviseGroup.Should().BeNull();

			var adviseGroup = await GetAdviseGroup(ReferenceData.FolderAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.SubFolder;
			expectedFolder.AdviseGroup = referenceData.FolderAdviseGroup;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.SubFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForFolderWithSingularAdviseGroup_AssignsNewAdviseGroupAndDeletesOldAdviseGroup()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Id.Should().Be(ReferenceData.FolderAdviseGroupId);

			var adviseGroup = await GetAdviseGroup(ReferenceData.DiscAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = referenceData.DiscAdviseGroup;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForFolderWithPluralAdviseGroup_AssignsNewAdviseGroupAndDoesNotDeleteOldAdviseGroup()
		{
			// Arrange

			var dirtyReferenceData = GetReferenceData();

			// Assigning one more reference to FolderAdviseGroup.
			var target = CreateTestTarget();
			await target.AssignAdviseGroup(dirtyReferenceData.SubFolder, dirtyReferenceData.FolderAdviseGroup, CancellationToken.None);

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Id.Should().Be(ReferenceData.FolderAdviseGroupId);

			var adviseGroup = await GetAdviseGroup(ReferenceData.DiscAdviseGroupId);

			// Act

			await target.AssignAdviseGroup(folder, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = referenceData.DiscAdviseGroup;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_IfFolderIsLastHolderOfAdviseGroup_DeletesAssignedAdviseGroup()
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

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_IfFolderIsNotLastHolderOfAdviseGroup_DoesNotDeleteAssignedAdviseGroup()
		{
			// Arrange

			var dirtyReferenceData = GetReferenceData();

			// Assigning one more reference to FolderAdviseGroup.
			var target = CreateTestTarget();
			await target.AssignAdviseGroup(dirtyReferenceData.SubFolder, dirtyReferenceData.FolderAdviseGroup, CancellationToken.None);

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Should().BeEquivalentTo(dirtyReferenceData.FolderAdviseGroup);

			// Act

			await target.RemoveAdviseGroup(folder, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();

			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = null;

			folder.Should().BeEquivalentTo(expectedFolder);

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

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
