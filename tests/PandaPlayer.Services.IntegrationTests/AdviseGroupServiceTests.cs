using System;
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

			newAdviseGroup.Should().BeEquivalentTo(expectedAdviseGroup, x => x.WithStrictOrdering());

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
		public async Task UpdateAdviseGroup_IfFavoriteStatusIsReversed_UpdatesAdviseGroupCorrectly()
		{
			// Arrange

			var adviseGroup = await GetAdviseGroup(ReferenceData.FolderAdviseGroupId);
			adviseGroup.IsFavorite.Should().BeFalse();

			var target = CreateTestTarget();

			// Act

			adviseGroup.IsFavorite = true;
			await target.UpdateAdviseGroup(adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			referenceData.FolderAdviseGroup.IsFavorite = true;

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

			folder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

			// TODO: After switching to in-memory DiscLibrary, folderFromRepository (and similar variables in other IT) are not applicable anymore.
			// It will reference the same object as folder and below check is equivalent to the one performed above.
			// So we need either to read new instance directly from the repository or remove this additional check at all.
			// The later could be done safely, because in-memory data is compared with database data in CheckLibraryConsistency().
			// The only argument against such removing is potential redesign in the future when in-memory models holder is abandoned.
			var folderFromRepository = await GetFolder(ReferenceData.SubFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

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

			folder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

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

			// Assigning one more reference to FolderAdviseGroup.
			await AssignAdviseGroupToFolder(ReferenceData.SubFolderId, ReferenceData.FolderAdviseGroupId);

			var assignedFolder = await GetFolder(ReferenceData.ArtistFolderId);
			assignedFolder.AdviseGroup.Id.Should().Be(ReferenceData.FolderAdviseGroupId);

			var assignedAdviseGroup = await GetAdviseGroup(ReferenceData.DiscAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(assignedFolder, assignedAdviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = referenceData.DiscAdviseGroup;

			assignedFolder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

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
		public async Task AssignAdviseGroup_ForDiscWithoutAdviseGroup_AssignsAdviseGroupCorrectly()
		{
			// Arrange

			var disc = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			disc.AdviseGroup.Should().BeNull();

			var adviseGroup = await GetAdviseGroup(ReferenceData.DiscAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(disc, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.DiscWithMissingFields;
			expectedDisc.AdviseGroup = referenceData.DiscAdviseGroup;

			disc.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.DiscWithMissingFieldsId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForDiscWithSingularAdviseGroup_AssignsNewAdviseGroupAndDeletesOldAdviseGroup()
		{
			// Arrange

			var disc = await GetDisc(ReferenceData.NormalDiscId);
			disc.AdviseGroup.Id.Should().Be(ReferenceData.DiscAdviseGroupId);

			var adviseGroup = await GetAdviseGroup(ReferenceData.FolderAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(disc, adviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.AdviseGroup = referenceData.FolderAdviseGroup;

			disc.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedAdviseGroups = new[]
			{
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task AssignAdviseGroup_ForDiscWithPluralAdviseGroup_AssignsNewAdviseGroupAndDoesNotDeleteOldAdviseGroup()
		{
			// Arrange

			// Assigning one more reference to DiscAdviseGroup.
			await AssignAdviseGroupToDisc(ReferenceData.DiscWithMissingFieldsId, ReferenceData.DiscAdviseGroupId);

			var assignedDisc = await GetDisc(ReferenceData.NormalDiscId);
			assignedDisc.AdviseGroup.Id.Should().Be(ReferenceData.DiscAdviseGroupId);

			var assignedAdviseGroup = await GetAdviseGroup(ReferenceData.FolderAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.AssignAdviseGroup(assignedDisc, assignedAdviseGroup, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.AdviseGroup = referenceData.FolderAdviseGroup;

			assignedDisc.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

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

			folder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

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

			// Assigning one more reference to FolderAdviseGroup.
			await AssignAdviseGroupToFolder(ReferenceData.SubFolderId, ReferenceData.FolderAdviseGroupId);

			var folder = await GetFolder(ReferenceData.ArtistFolderId);
			folder.AdviseGroup.Id.Should().Be(ReferenceData.FolderAdviseGroupId);

			var target = CreateTestTarget();

			// Act

			await target.RemoveAdviseGroup(folder, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();

			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.AdviseGroup = null;

			folder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering());

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
		public async Task RemoveAdviseGroup_IfDiscIsLastHolderOfAdviseGroup_DeletesAssignedAdviseGroup()
		{
			// Arrange

			var disc = await GetDisc(ReferenceData.NormalDiscId);
			disc.AdviseGroup.Should().NotBeNull();

			var target = CreateTestTarget();

			// Act

			await target.RemoveAdviseGroup(disc, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.AdviseGroup = null;

			disc.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedAdviseGroups = new[]
			{
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task RemoveAdviseGroup_IfDiscIsNotLastHolderOfAdviseGroup_DoesNotDeleteAssignedAdviseGroup()
		{
			// Arrange

			// Assigning one more reference to DiscAdviseGroup.
			await AssignAdviseGroupToDisc(ReferenceData.DiscWithMissingFieldsId, ReferenceData.DiscAdviseGroupId);

			var disc = await GetDisc(ReferenceData.NormalDiscId);
			disc.AdviseGroup.Id.Should().BeEquivalentTo(ReferenceData.DiscAdviseGroupId, x => x.WithStrictOrdering());

			var target = CreateTestTarget();

			// Act

			await target.RemoveAdviseGroup(disc, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();

			var expectedDisc = referenceData.NormalDisc;
			expectedDisc.AdviseGroup = null;

			disc.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var discFromRepository = await GetDisc(ReferenceData.NormalDiscId);
			discFromRepository.Should().BeEquivalentTo(expectedDisc, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await target.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		private async Task AssignAdviseGroupToDisc(ItemId discId, ItemId adviseGroupId)
		{
			var disc = await GetDisc(discId);
			var adviseGroup = await GetAdviseGroup(adviseGroupId);

			var adviseGroupService = GetService<IAdviseGroupService>();
			await adviseGroupService.AssignAdviseGroup(disc, adviseGroup, CancellationToken.None);
		}
	}
}
