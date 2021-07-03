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
				ParentFolderId = new ItemId("1"),
				Name = "Test Folder",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(new ItemId("6"));

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Test Folder");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Count.Should().Be(6);

			var testData = GetTestData();
			var expectedFolder = new FolderModel
			{
				Id = new ItemId("6"),
				ParentFolderId = new ItemId("1"),
				ParentFolder = testData.RootFolder,
				Name = "Test Folder",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var createdFolder = await target.GetFolder(new ItemId("6"), CancellationToken.None);
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
				ParentFolderId = new ItemId("3"),
				Name = "Test Folder",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(new ItemId("6"));

			var expectedFolderPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Test Folder");
			Directory.Exists(expectedFolderPath).Should().BeTrue();

			var allFolders = await target.GetAllFolders(CancellationToken.None);
			allFolders.Count.Should().Be(6);

			var testData = GetTestData();
			var expectedFolder = new FolderModel
			{
				Id = new ItemId("6"),
				ParentFolderId = new ItemId("3"),
				ParentFolder = testData.ArtistFolder,
				Name = "Test Folder",
				Subfolders = new List<ShallowFolderModel>(),
				Discs = new List<DiscModel>(),
			};

			var createdFolder = await target.GetFolder(new ItemId("6"), CancellationToken.None);
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
				ParentFolderId = new ItemId("3"),
				Name = "Foreign",
			};

			await target.CreateFolder(newFolder, CancellationToken.None);

			// Assert

			newFolder.Id.Should().Be(new ItemId("6"));

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
				ParentFolderId = new ItemId("2"),
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

			var testData = GetTestData();
			var expectedFolders = new[]
			{
				testData.RootFolder,
				testData.SubFolder,
				testData.ArtistFolder,
				testData.EmptyFolder,
				testData.DeletedFolder,
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

			var testData = GetTestData();
			var expectedFolder = new FolderModel
			{
				Id = new ItemId("1"),
				Name = "<ROOT>",
				Subfolders = new List<ShallowFolderModel>
				{
					testData.SubFolder,
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

			var folder = await target.GetFolder(new ItemId("1"), CancellationToken.None);

			// Assert

			var testData = GetTestData();
			var expectedFolder = new FolderModel
			{
				Id = new ItemId("1"),
				Name = "<ROOT>",
				Subfolders = new List<ShallowFolderModel>
				{
					testData.SubFolder,
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

			var folder = await target.GetFolder(new ItemId("3"), CancellationToken.None);

			// Assert

			var testData = GetTestData();
			var expectedFolder = new FolderModel
			{
				Id = new ItemId("3"),
				ParentFolderId = new ItemId("2"),
				ParentFolder = testData.SubFolder,
				Name = "Guano Apes",
				Subfolders = new List<ShallowFolderModel>
				{
					testData.EmptyFolder,
					testData.DeletedFolder,
				},
				Discs = new List<DiscModel>
				{
					testData.NormalDisc,
					testData.DiscWithNullValues,
					testData.DeletedDisc,
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

			await target.DeleteFolder(new ItemId("4"), CancellationToken.None);

			// Assert

			var foldersAfterDeletion = await target.GetAllFolders(CancellationToken.None);

			var testData = GetTestData();
			var expectedFolders = new[]
			{
				testData.RootFolder,
				testData.SubFolder,
				testData.ArtistFolder,
				new ShallowFolderModel
				{
					Id = new ItemId("4"),
					ParentFolderId = new ItemId("3"),
					Name = "Empty Folder",
					DeleteDate = new DateTimeOffset(2021, 07, 02, 18, 49, 51, TimeSpan.FromHours(3)),
				},
				testData.DeletedFolder,
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

			Func<Task> call = () => target.DeleteFolder(new ItemId("3"), CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>();
		}
	}
}
