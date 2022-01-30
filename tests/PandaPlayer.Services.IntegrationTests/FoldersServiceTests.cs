using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.IntegrationTests.Data;
using PandaPlayer.Services.IntegrationTests.Extensions;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.Services.IntegrationTests
{
	[TestClass]
	public class FoldersServiceTests : BasicServiceTests<IFoldersService>
	{
		[TestMethod]
		public async Task CreateEmptyFolder_ForRootParent_CreatesFolderCorrectly()
		{
			// Arrange

			var newFolder = new FolderModel
			{
				Name = "Ляпис Трубецкой",
			};

			var parentFolder = await GetFolder(ReferenceData.RootFolderId);
			parentFolder.AddSubfolder(newFolder);

			var target = CreateTestTarget();

			// Act

			await target.CreateEmptyFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				Name = "Ляпис Трубецкой",
			};

			var referenceData = GetReferenceData();
			referenceData.RootFolder.AddSubfolder(expectedFolder);

			newFolder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedFolder,
			};

			var allFolders = await GetAllFolders();
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var folderFromRepository = await GetFolder(ReferenceData.NextFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Ляпис Трубецкой");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateEmptyFolder_ForNonRootParent_CreatesFolderCorrectly()
		{
			// Arrange

			var newFolder = new FolderModel
			{
				Name = "Ляпис Трубецкой",
			};

			var parentFolder = await GetFolder(ReferenceData.ArtistFolderId);
			parentFolder.AddSubfolder(newFolder);

			var target = CreateTestTarget();

			// Act

			await target.CreateEmptyFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				Name = "Ляпис Трубецкой",
			};

			var referenceData = GetReferenceData();
			referenceData.ArtistFolder.AddSubfolder(expectedFolder);

			newFolder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedFolder,
			};

			var allFolders = await GetAllFolders();
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var folderFromRepository = await GetFolder(ReferenceData.NextFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Ляпис Трубецкой");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task CreateEmptyFolder_IfFolderWithSameNameExistsForOtherParent_CreatesFolderSuccessfully()
		{
			// Arrange

			var newFolder = new FolderModel
			{
				Name = "Belarusian",
			};

			var parentFolder = await GetFolder(ReferenceData.ArtistFolderId);
			parentFolder.AddSubfolder(newFolder);

			var target = CreateTestTarget();

			// Act

			await target.CreateEmptyFolder(newFolder, CancellationToken.None);

			// Assert

			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				Name = "Belarusian",
			};

			var referenceData = GetReferenceData();
			referenceData.ArtistFolder.AddSubfolder(expectedFolder);

			newFolder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
				expectedFolder,
			};

			var allFolders = await GetAllFolders();
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var folderFromRepository = await GetFolder(ReferenceData.NextFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Belarusian");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			await CheckLibraryConsistency();
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
			folder.Should().BeEquivalentTo(referenceData.RootFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateFolder_IfNonEmptyFolderIsRenamed_RenamesFolderCorrectly()
		{
			// Arrange

			var folder = await GetFolder(ReferenceData.ArtistFolderId);

			var target = CreateTestTarget();

			// Act

			void UpdateFolder(FolderModel folder)
			{
				folder.Name = "New Folder Name";
			}

			await target.UpdateFolder(folder, UpdateFolder, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedFolder = referenceData.ArtistFolder;
			expectedFolder.Name = "New Folder Name";

			var songs1 = referenceData.NormalDisc.ActiveSongs.ToList();
			songs1[0].ContentUri = "Belarusian/New Folder Name/2010 - Афтары правды (CD 1)/01 - Про женщин.mp3".ToContentUri(LibraryStorageRoot);
			songs1[1].ContentUri = "Belarusian/New Folder Name/2010 - Афтары правды (CD 1)/02 - Про жизнь дяди Саши.mp3".ToContentUri(LibraryStorageRoot);
			referenceData.NormalDisc.Images.Single().ContentUri = "Belarusian/New Folder Name/2010 - Афтары правды (CD 1)/cover.jpg".ToContentUri(LibraryStorageRoot);

			var songs2 = referenceData.DiscWithMissingFields.ActiveSongs.ToList();
			songs2[0].ContentUri = "Belarusian/New Folder Name/Disc With Missing Fields (CD 1)/Song With Missing Fields.mp3".ToContentUri(LibraryStorageRoot);

			folder.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var folderFromRepository = await GetFolder(ReferenceData.ArtistFolderId);
			folderFromRepository.Should().BeEquivalentTo(expectedFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var oldDirectoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel");
			Directory.Exists(oldDirectoryPath).Should().BeFalse();

			var newDirectoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "New Folder Name");
			Directory.Exists(newDirectoryPath).Should().BeTrue();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteEmptyFolder_ForEmptyFolder_DeletesFolderSuccessfully()
		{
			// Arrange

			var directoryPath = Path.Combine(LibraryStorageRoot, "Belarusian", "Neuro Dubel", "Empty Folder");
			Directory.Exists(directoryPath).Should().BeTrue();

			var target = CreateTestTarget(StubClock(new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3))));

			var deletedFolder = await GetFolder(ReferenceData.EmptyFolderId);

			// Act

			await target.DeleteEmptyFolder(deletedFolder, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			referenceData.EmptyFolder.DeleteDate = new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3));

			deletedFolder.Should().BeEquivalentTo(referenceData.EmptyFolder, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			var expectedFolders = new[]
			{
				referenceData.RootFolder,
				referenceData.SubFolder,
				referenceData.ArtistFolder,
				referenceData.EmptyFolder,
				referenceData.DeletedFolder,
			};

			var allFolders = await GetAllFolders();
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			Directory.Exists(directoryPath).Should().BeFalse();

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteEmptyFolder_IfFolderIsLastHolderOfAdviseGroup_DeletesAssignedAdviseGroup()
		{
			// Arrange

			// Creating advise group which will be assigned only to EmptyFolder.
			var adviseGroup = new AdviseGroupModel
			{
				Name = "Test Advise Group",
			};

			var holdingFolder = await GetFolder(ReferenceData.EmptyFolderId);

			var adviseGroupService = GetService<IAdviseGroupService>();
			await adviseGroupService.CreateAdviseGroup(adviseGroup, CancellationToken.None);
			await adviseGroupService.AssignAdviseGroup(holdingFolder, adviseGroup, CancellationToken.None);

			var deletedFolder = await GetFolder(ReferenceData.EmptyFolderId);

			var target = CreateTestTarget();

			// Act

			await target.DeleteEmptyFolder(deletedFolder, CancellationToken.None);

			// Assert

			var folderFromRepository = await GetFolder(ReferenceData.EmptyFolderId);
			folderFromRepository.AdviseGroup.Should().BeNull();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await adviseGroupService.GetAllAdviseGroups(CancellationToken.None);
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteEmptyFolder_IfFolderIsNotLastHolderOfAdviseGroup_DoesNotDeletesAssignedAdviseGroup()
		{
			// Arrange

			// Assigning one more reference to FolderAdviseGroup.
			await AssignAdviseGroupToFolder(ReferenceData.EmptyFolderId, ReferenceData.FolderAdviseGroupId);

			var deletedFolder = await GetFolder(ReferenceData.EmptyFolderId);

			var target = CreateTestTarget();

			// Act

			await target.DeleteEmptyFolder(deletedFolder, CancellationToken.None);

			// Assert

			var folderFromRepository = await GetFolder(ReferenceData.EmptyFolderId);
			folderFromRepository.AdviseGroup.Should().BeNull();

			var referenceData = GetReferenceData();
			var expectedAdviseGroups = new[]
			{
				referenceData.DiscAdviseGroup,
				referenceData.FolderAdviseGroup,
			};

			var adviseGroups = await GetAllAdviseGroups();
			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithStrictOrdering());

			await CheckLibraryConsistency();
		}

		[TestMethod]
		public async Task DeleteEmptyFolder_ForNonEmptyFolder_ThrowsInvalidOperationException()
		{
			// Arrange

			var deletedFolder = await GetFolder(ReferenceData.ArtistFolderId);

			var target = CreateTestTarget();

			// Act

			var call = () => target.DeleteEmptyFolder(deletedFolder, CancellationToken.None);

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

			var allFolders = await GetAllFolders();
			allFolders.Should().BeEquivalentTo(expectedFolders, x => x.WithStrictOrdering().IgnoringCyclicReferences());

			await CheckLibraryConsistency();
		}
	}
}
