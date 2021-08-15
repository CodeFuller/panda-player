using System;
using System.Collections.Generic;
using System.IO;
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
	public class FoldersServiceTests : BasicServiceTests<IFoldersService>
	{
		[TestMethod]
		public async Task CreateFolder_ForRootParent_CreatesFolderCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var newFolder = new ShallowFolderModel
			{
				ParentFolderId = ReferenceData.RootFolderId,
				Name = "Ляпис Трубецкой",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedShallowFolder = new ShallowFolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.RootFolderId,
				Name = "Ляпис Трубецкой",
			};

			newFolder.Should().BeEquivalentTo(expectedShallowFolder);

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedShallowFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.IgnoringCyclicReferences());

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.RootFolderId,
				ParentFolder = referenceData.RootFolder,
				Name = "Ляпис Трубецкой",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var folderFromRepository = await target.GetFolder(ReferenceData.NextFolderId, CancellationToken.None);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Ляпис Трубецкой");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateFolder_ForNonRootParent_CreatesFolderCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var newFolder = new ShallowFolderModel
			{
				ParentFolderId = ReferenceData.ArtistFolderId,
				Name = "Ляпис Трубецкой",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedShallowFolder = new ShallowFolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.ArtistFolderId,
				Name = "Ляпис Трубецкой",
			};

			newFolder.Id.Should().Be(ReferenceData.NextFolderId);

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedShallowFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.IgnoringCyclicReferences());

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.ArtistFolderId,
				ParentFolder = referenceData.ArtistFolder,
				Name = "Ляпис Трубецкой",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var folderFromRepository = await target.GetFolder(ReferenceData.NextFolderId, CancellationToken.None);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Ляпис Трубецкой");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateFolder_IfFolderWithSameNameExistsForOtherParent_CreatesFolderSuccessfully()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var newFolder = new ShallowFolderModel
			{
				ParentFolderId = ReferenceData.ArtistFolderId,
				Name = "Belarusian",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedShallowFolder = new ShallowFolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.ArtistFolderId,
				Name = "Belarusian",
			};

			newFolder.Id.Should().Be(ReferenceData.NextFolderId);

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedShallowFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.IgnoringCyclicReferences());

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.ArtistFolderId,
				ParentFolder = referenceData.ArtistFolder,
				Name = "Belarusian",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var folderFromRepository = await target.GetFolder(ReferenceData.NextFolderId, CancellationToken.None);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Belarusian");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateFolder_IfFolderWithSameNameExistsForParent_Throws()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var newFolder = new ShallowFolderModel
			{
				ParentFolderId = ReferenceData.SubFolderId,
				Name = "Neuro Dubel",
			};

			Func<Task> call = () => target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task GetAllFolders_ReturnsFoldersCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var folders = await target.GetAllFolders(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
			};

			folders.Should().BeEquivalentTo(expectedFolders);
		}

		[TestMethod]
		public async Task GetRootFolder_ReturnsRootFolderCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var folder = await target.GetRootFolder(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.RootFolderId,
				Name = "<ROOT>",
				Subfolders = new List<ShallowFolderModel>
				{
					referenceData.SubFolder,
				},
				Discs = new List<DiscModel>(),
			};

			folder.Should().BeEquivalentTo(expectedFolder);
		}

		[TestMethod]
		public async Task GetFolder_ForRootFolderId_ReturnsFolderCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var folder = await target.GetFolder(ReferenceData.RootFolderId, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.RootFolderId,
				Name = "<ROOT>",
				Subfolders = new List<ShallowFolderModel>
				{
					referenceData.SubFolder,
				},
				Discs = new List<DiscModel>(),
			};

			folder.Should().BeEquivalentTo(expectedFolder);
		}

		[TestMethod]
		public async Task GetFolder_ForNonRootFolderId_ReturnsFolderCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var folder = await target.GetFolder(ReferenceData.ArtistFolderId, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.ArtistFolderId,
				ParentFolderId = ReferenceData.SubFolderId,
				ParentFolder = referenceData.SubFolder,
				Name = "Neuro Dubel",
				AdviseGroup = referenceData.AdviseGroup1,
				Subfolders = new List<ShallowFolderModel>
				{
					referenceData.EmptyFolder,
					referenceData.DeletedFolder,
				},
				Discs = new List<DiscModel>
				{
					referenceData.NormalDisc,
					referenceData.DiscWithMissingFields,
					referenceData.DeletedDisc,
				},
			};

			folder.Should().BeEquivalentTo(expectedFolder, x => x.IgnoringCyclicReferences());
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

		[TestMethod]
		public async Task DeleteFolder_ForEmptyFolder_DeletesFolderSuccessfully()
		{
			// Arrange

			var target = CreateTestTarget(StubClock(new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3))));

			var directoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Empty Folder");
			Directory.Exists(directoryPath).Should().BeTrue();

			// Act

			await target.DeleteFolder(ReferenceData.EmptyFolderId, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				new ShallowFolderModel
				{
					Id = ReferenceData.EmptyFolderId,
					ParentFolderId = ReferenceData.ArtistFolderId,
					Name = "Empty Folder",
					DeleteDate = new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3)),
				},
				referenceData.DeletedFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders);

			Directory.Exists(directoryPath).Should().BeFalse();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteFolder_ForNonEmptyFolder_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			Func<Task> call = () => target.DeleteFolder(ReferenceData.ArtistFolderId, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>();

			var referenceData = GetReferenceData();
			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
			};

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.IgnoringCyclicReferences());

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
