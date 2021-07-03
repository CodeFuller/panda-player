using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Services.IntegrationTests.Data;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.IntegrationTests
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
				Name = "Test Folder",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(ReferenceData.NextFolderId);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Test Folder");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Count.Should().Be(6);

			var referenceData = GetReferenceData();
			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.RootFolderId,
				ParentFolder = referenceData.RootFolder,
				Name = "Test Folder",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var createdFolder = await target.GetFolder(ReferenceData.NextFolderId, CancellationToken.None);
			createdFolder.Should().BeEquivalentTo(expectedFolder);
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
				Name = "Test Folder",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(ReferenceData.NextFolderId);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Test Folder");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Count.Should().Be(6);

			var referenceData = GetReferenceData();
			var expectedFolder = new FolderModel
			{
				Id = ReferenceData.NextFolderId,
				ParentFolderId = ReferenceData.ArtistFolderId,
				ParentFolder = referenceData.ArtistFolder,
				Name = "Test Folder",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var createdFolder = await target.GetFolder(ReferenceData.NextFolderId, CancellationToken.None);
			createdFolder.Should().BeEquivalentTo(expectedFolder);
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
				Name = "Foreign",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(ReferenceData.NextFolderId);

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Foreign");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Count.Should().Be(6);
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
				Name = "Guano Apes",
			};

			Func<Task> call = () => target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<DbUpdateException>();
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

			folders.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedFolders);
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
				Name = "Guano Apes",
				Subfolders = new List<ShallowFolderModel>
				{
					referenceData.EmptyFolder,
					referenceData.DeletedFolder,
				},
				Discs = new List<DiscModel>
				{
					referenceData.NormalDisc,
					referenceData.DiscWithNullValues,
					referenceData.DeletedDisc,
				},
			};

			folder.Should().BeEquivalentTo(expectedFolder, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task DeleteFolder_ForEmptyFolder_DeletesFolderSuccessfully()
		{
			// Arrange

			var target = CreateTestTarget(StubClock(new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3))));

			// There is no trivial way to provide empty directory with Git.
			var directoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Empty Folder");
			Directory.CreateDirectory(directoryPath);

			// Act

			await target.DeleteFolder(ReferenceData.EmptyFolderId, CancellationToken.None);

			// Assert

			var foldersAfterDeletion = await target.GetAllFolders(CancellationToken.None);

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

			foldersAfterDeletion.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedFolders);

			Directory.Exists(directoryPath).Should().BeFalse();
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
		}
	}
}
