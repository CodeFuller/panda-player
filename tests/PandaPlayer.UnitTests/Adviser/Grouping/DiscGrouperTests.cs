using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.UnitTests.Adviser.Grouping
{
	[TestClass]
	public class DiscGrouperTests
	{
		[TestMethod]
		public async Task GroupLibraryDiscs_ForDiscsBelongingToSameAdviseGroup_AssignsDiscsToSameDiscGroup()
		{
			// Arrange

			var folderAdviseGroup = new AdviseGroupModel { Id = new ItemId("Folder Advise Group") };
			var discAdviseGroup1 = new AdviseGroupModel { Id = new ItemId("Disc Advise Group 1") };
			var discAdviseGroup2 = new AdviseGroupModel { Id = new ItemId("Disc Advise Group 2") };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), ParentFolderId = null };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), ParentFolderId = rootFolder.Id };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), ParentFolderId = rootFolder.Id };

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), ParentFolderId = folder1.Id, AdviseGroup = folderAdviseGroup };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), ParentFolderId = folder2.Id, AdviseGroup = folderAdviseGroup };

			var disc11 = new DiscModel { Folder = folder11, AdviseGroup = discAdviseGroup1, AllSongs = Array.Empty<SongModel>() };
			var disc12 = new DiscModel { Folder = folder11, AdviseGroup = discAdviseGroup1, AllSongs = Array.Empty<SongModel>() };
			var disc21 = new DiscModel { Folder = folder21, AdviseGroup = discAdviseGroup2, AllSongs = Array.Empty<SongModel>() };

			var folderServiceStub = new Mock<IFoldersService>();
			folderServiceStub.Setup(x => x.GetAllFolders(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { rootFolder, folder1, folder2, folder11, folder21 });

			var mocker = new AutoMocker();
			mocker.Use(folderServiceStub);

			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var discGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedDiscGroup1 = new DiscGroup("Advise Group: Disc Advise Group 1");
			expectedDiscGroup1.AddDisc(disc11);
			expectedDiscGroup1.AddDisc(disc12);

			var expectedDiscGroup2 = new DiscGroup("Advise Group: Disc Advise Group 2");
			expectedDiscGroup2.AddDisc(disc21);

			var expectedGroups = new[]
			{
				expectedDiscGroup1,
				expectedDiscGroup2,
			};

			discGroups.Should().BeEquivalentTo(expectedGroups);
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_ForDiscsFromFolderBelongingToAdviseGroup_AssignsDiscsToSameDiscGroup()
		{
			// Arrange

			var rootAdviseGroup = new AdviseGroupModel { Id = new ItemId("Advise Group Root") };
			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("Advise Group 1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("Advise Group 2") };
			var adviseGroup11 = new AdviseGroupModel { Id = new ItemId("Advise Group 11") };
			var adviseGroup21 = new AdviseGroupModel { Id = new ItemId("Advise Group 21") };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), ParentFolderId = null, AdviseGroup = rootAdviseGroup };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), ParentFolderId = rootFolder.Id, AdviseGroup = adviseGroup1 };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), ParentFolderId = rootFolder.Id, AdviseGroup = adviseGroup2 };

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), ParentFolderId = folder1.Id, AdviseGroup = adviseGroup11 };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), ParentFolderId = folder2.Id, AdviseGroup = adviseGroup21 };

			var disc11 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc12 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc21 = new DiscModel { Folder = folder21, AllSongs = Array.Empty<SongModel>() };

			var folderServiceStub = new Mock<IFoldersService>();
			folderServiceStub.Setup(x => x.GetAllFolders(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { rootFolder, folder1, folder2, folder11, folder21 });

			var mocker = new AutoMocker();
			mocker.Use(folderServiceStub);

			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var discGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedDiscGroup1 = new DiscGroup("Advise Group: Advise Group 11");
			expectedDiscGroup1.AddDisc(disc11);
			expectedDiscGroup1.AddDisc(disc12);

			var expectedDiscGroup2 = new DiscGroup("Advise Group: Advise Group 21");
			expectedDiscGroup2.AddDisc(disc21);

			var expectedGroups = new[]
			{
				expectedDiscGroup1,
				expectedDiscGroup2,
			};

			discGroups.Should().BeEquivalentTo(expectedGroups);
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_IfSomeUpperFolderBelongsToAdviseGroup_AssignsDiscsToAdviseGroupFromClosestFolder()
		{
			// Arrange

			var rootAdviseGroup = new AdviseGroupModel { Id = new ItemId("Advise Group Root") };
			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("Advise Group 1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("Advise Group 2") };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), ParentFolderId = null, AdviseGroup = rootAdviseGroup };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), ParentFolderId = rootFolder.Id, AdviseGroup = adviseGroup1 };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), ParentFolderId = rootFolder.Id, AdviseGroup = adviseGroup2 };

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), ParentFolderId = folder1.Id };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), ParentFolderId = folder2.Id };

			var disc11 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc12 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc21 = new DiscModel { Folder = folder21, AllSongs = Array.Empty<SongModel>() };

			var folderServiceStub = new Mock<IFoldersService>();
			folderServiceStub.Setup(x => x.GetAllFolders(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { rootFolder, folder1, folder2, folder11, folder21 });

			var mocker = new AutoMocker();
			mocker.Use(folderServiceStub);

			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var discGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedDiscGroup1 = new DiscGroup("Advise Group: Advise Group 1");
			expectedDiscGroup1.AddDisc(disc11);
			expectedDiscGroup1.AddDisc(disc12);

			var expectedDiscGroup2 = new DiscGroup("Advise Group: Advise Group 2");
			expectedDiscGroup2.AddDisc(disc21);

			var expectedGroups = new[]
			{
				expectedDiscGroup1,
				expectedDiscGroup2,
			};

			discGroups.Should().BeEquivalentTo(expectedGroups);
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_IfNoFolderBelongsToAssignedGroupUpToRoot_AssignsDiscsFromSameFolderToOneGroup()
		{
			// Arrange

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), ParentFolderId = null };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), ParentFolderId = rootFolder.Id };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), ParentFolderId = rootFolder.Id };

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), ParentFolderId = folder1.Id };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), ParentFolderId = folder2.Id };

			var disc11 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc12 = new DiscModel { Folder = folder11, AllSongs = Array.Empty<SongModel>() };
			var disc21 = new DiscModel { Folder = folder21, AllSongs = Array.Empty<SongModel>() };

			var folderServiceStub = new Mock<IFoldersService>();
			folderServiceStub.Setup(x => x.GetAllFolders(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { rootFolder, folder1, folder2, folder11, folder21 });

			var mocker = new AutoMocker();
			mocker.Use(folderServiceStub);

			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var discGroups = await target.GroupLibraryDiscs(new[] { disc11, disc21, disc12 }, CancellationToken.None);

			// Assert

			var expectedDiscGroup1 = new DiscGroup("Folder Group: Folder 11");
			expectedDiscGroup1.AddDisc(disc11);
			expectedDiscGroup1.AddDisc(disc12);

			var expectedDiscGroup2 = new DiscGroup("Folder Group: Folder 21");
			expectedDiscGroup2.AddDisc(disc21);

			var expectedGroups = new[]
			{
				expectedDiscGroup1,
				expectedDiscGroup2,
			};

			discGroups.Should().BeEquivalentTo(expectedGroups);
		}
	}
}
