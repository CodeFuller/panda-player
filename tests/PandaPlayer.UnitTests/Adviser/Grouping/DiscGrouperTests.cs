using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser.Grouping
{
	[TestClass]
	public class DiscGrouperTests
	{
		[TestMethod]
		public async Task GroupLibraryDiscs_ForDiscsBelongingToSameAdviseGroup_AssignsDiscsToSameAdviseGroupContent()
		{
			// Arrange

			var folderAdviseGroup = new AdviseGroupModel { Id = new ItemId("Folder Advise Group") };
			var discAdviseGroup1 = new AdviseGroupModel { Id = new ItemId("Disc Advise Group 1") };
			var discAdviseGroup2 = new AdviseGroupModel { Id = new ItemId("Disc Advise Group 2"), IsFavorite = true };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder") };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), Name = "Folder 1" };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), Name = "Folder 2" };
			rootFolder.AddSubfolders(folder1, folder2);

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), AdviseGroup = folderAdviseGroup };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), AdviseGroup = folderAdviseGroup };
			folder1.AddSubfolders(folder11);
			folder2.AddSubfolders(folder21);

			var disc11 = new DiscModel { AdviseGroup = discAdviseGroup1 };
			var disc12 = new DiscModel { AdviseGroup = discAdviseGroup1 };
			var disc21 = new DiscModel { AdviseGroup = discAdviseGroup2 };
			folder11.AddDiscs(disc11, disc12);
			folder21.AddDiscs(disc21);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var adviseGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedAdviseGroup1 = new AdviseGroupContent("Advise Group: Disc Advise Group 1", isFavorite: false);
			expectedAdviseGroup1.AddDisc(disc11);
			expectedAdviseGroup1.AddDisc(disc12);

			var expectedAdviseGroup2 = new AdviseGroupContent("Advise Group: Disc Advise Group 2", isFavorite: true);
			expectedAdviseGroup2.AddDisc(disc21);

			var expectedAdviseGroups = new[]
			{
				expectedAdviseGroup1,
				expectedAdviseGroup2,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_ForDiscsFromFolderBelongingToAdviseGroup_AssignsDiscsToSameAdviseGroupContent()
		{
			// Arrange

			var rootAdviseGroup = new AdviseGroupModel { Id = new ItemId("Advise Group Root") };
			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("Advise Group 1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("Advise Group 2") };
			var adviseGroup11 = new AdviseGroupModel { Id = new ItemId("Advise Group 11") };
			var adviseGroup21 = new AdviseGroupModel { Id = new ItemId("Advise Group 21"), IsFavorite = true };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), AdviseGroup = rootAdviseGroup };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), Name = "Folder 1", AdviseGroup = adviseGroup1 };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), Name = "Folder 2", AdviseGroup = adviseGroup2 };
			rootFolder.AddSubfolders(folder1, folder2);

			var folder11 = new FolderModel { Id = new ItemId("Folder 11"), AdviseGroup = adviseGroup11 };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21"), AdviseGroup = adviseGroup21 };
			folder1.AddSubfolders(folder11);
			folder2.AddSubfolders(folder21);

			var disc11 = new DiscModel();
			var disc12 = new DiscModel();
			var disc21 = new DiscModel();
			folder11.AddDiscs(disc11, disc12);
			folder21.AddDiscs(disc21);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var adviseGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedAdviseGroup1 = new AdviseGroupContent("Advise Group: Advise Group 11", isFavorite: false);
			expectedAdviseGroup1.AddDisc(disc11);
			expectedAdviseGroup1.AddDisc(disc12);

			var expectedAdviseGroup2 = new AdviseGroupContent("Advise Group: Advise Group 21", isFavorite: true);
			expectedAdviseGroup2.AddDisc(disc21);

			var expectedAdviseGroups = new[]
			{
				expectedAdviseGroup1,
				expectedAdviseGroup2,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_IfSomeUpperFolderBelongsToAdviseGroup_AssignsDiscsToAdviseGroupFromClosestFolder()
		{
			// Arrange

			var rootAdviseGroup = new AdviseGroupModel { Id = new ItemId("Advise Group Root") };
			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("Advise Group 1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("Advise Group 2"), IsFavorite = true };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder"), AdviseGroup = rootAdviseGroup };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), Name = "Folder 1", AdviseGroup = adviseGroup1 };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), Name = "Folder 2", AdviseGroup = adviseGroup2 };
			rootFolder.AddSubfolders(folder1, folder2);

			var folder11 = new FolderModel { Id = new ItemId("Folder 11") };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21") };
			folder1.AddSubfolders(folder11);
			folder2.AddSubfolders(folder21);

			var disc11 = new DiscModel();
			var disc12 = new DiscModel();
			var disc21 = new DiscModel();
			folder11.AddDiscs(disc11, disc12);
			folder21.AddDiscs(disc21);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var adviseGroups = await target.GroupLibraryDiscs(new[] { disc11, disc12, disc21 }, CancellationToken.None);

			// Assert

			var expectedAdviseGroup1 = new AdviseGroupContent("Advise Group: Advise Group 1", isFavorite: false);
			expectedAdviseGroup1.AddDisc(disc11);
			expectedAdviseGroup1.AddDisc(disc12);

			var expectedAdviseGroup2 = new AdviseGroupContent("Advise Group: Advise Group 2", isFavorite: true);
			expectedAdviseGroup2.AddDisc(disc21);

			var expectedAdviseGroups = new[]
			{
				expectedAdviseGroup1,
				expectedAdviseGroup2,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_IfNoFolderBelongsToAssignedGroupUpToRoot_AssignsDiscsFromSameFolderToOneAdviseGroupContent()
		{
			// Arrange

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder") };

			var folder1 = new FolderModel { Id = new ItemId("Folder 1"), Name = "Folder 1" };
			var folder2 = new FolderModel { Id = new ItemId("Folder 2"), Name = "Folder 2" };
			rootFolder.AddSubfolders(folder1, folder2);

			var folder11 = new FolderModel { Id = new ItemId("Folder 11") };
			var folder21 = new FolderModel { Id = new ItemId("Folder 21") };
			folder1.AddSubfolders(folder11);
			folder2.AddSubfolders(folder21);

			var disc11 = new DiscModel();
			var disc12 = new DiscModel();
			var disc21 = new DiscModel();
			folder11.AddDiscs(disc11, disc12);
			folder21.AddDiscs(disc21);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var adviseGroups = await target.GroupLibraryDiscs(new[] { disc11, disc21, disc12 }, CancellationToken.None);

			// Assert

			var expectedAdviseGroup1 = new AdviseGroupContent("Folder Group: Folder 11", isFavorite: false);
			expectedAdviseGroup1.AddDisc(disc11);
			expectedAdviseGroup1.AddDisc(disc12);

			var expectedAdviseGroup2 = new AdviseGroupContent("Folder Group: Folder 21", isFavorite: false);
			expectedAdviseGroup2.AddDisc(disc21);

			var expectedAdviseGroups = new[]
			{
				expectedAdviseGroup1,
				expectedAdviseGroup2,
			};

			adviseGroups.Should().BeEquivalentTo(expectedAdviseGroups, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public async Task GroupLibraryDiscs_IfDiscsBelongToSameAdviseSet_AssignsDiscsToSameAdviseSetContent()
		{
			// Arrange

			var adviseSet1 = new AdviseSetModel { Id = new ItemId("Advise Set 1") };
			var adviseSet2 = new AdviseSetModel { Id = new ItemId("Advise Set 2") };

			var disc11 = new DiscModel { AdviseSetInfo = new AdviseSetInfo(adviseSet1, 1) };
			var disc12 = new DiscModel { AdviseSetInfo = new AdviseSetInfo(adviseSet1, 2) };
			var disc21 = new DiscModel { AdviseSetInfo = new AdviseSetInfo(adviseSet2, 1) };

			var rootFolder = new FolderModel { Id = new ItemId("Root Folder") };
			rootFolder.AddDiscs(disc11, disc12, disc21);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscGrouper>();

			// Act

			var adviseGroups = await target.GroupLibraryDiscs(new[] { disc11, disc21, disc12 }, CancellationToken.None);

			// Assert

			adviseGroups.Count.Should().Be(1);

			var adviseSets = adviseGroups.Single().AdviseSets.ToList();
			adviseSets.Count.Should().Be(2);

			adviseSets[0].Discs.Should().BeEquivalentTo(new[] { disc11, disc12 }, x => x.WithStrictOrdering());
			adviseSets[1].Discs.Should().BeEquivalentTo(new[] { disc21 }, x => x.WithStrictOrdering());
		}
	}
}
