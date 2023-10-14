using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels.AdviseSetsEditor;

namespace PandaPlayer.UnitTests.ViewModels.AdviseSetsEditor
{
	[TestClass]
	public class AvailableDiscsViewModelTests
	{
		[TestMethod]
		public async Task LoadDiscs_ForDiscsWithAdviseSetAssigned_DoesNotAddSuchDiscsToList()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1" },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3" },
			};

			var folder1 = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			var folder2 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			var folder3 = new FolderModel { Id = new ItemId("2"), Name = "Folder 2" };
			folder1.AddSubfolders(folder2);
			folder2.AddSubfolders(folder3);

			folder2.AddDiscs(discs[0]);
			folder1.AddDiscs(discs[1]);
			folder3.AddDiscs(discs[2]);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			// Act

			await target.LoadDiscs(discs, CancellationToken.None);

			// Assert

			var expectedAvailableDiscs = new[]
			{
				new AvailableDiscViewModel(discs[0], "/Folder 1/Disc 1"),
				new AvailableDiscViewModel(discs[2], "/Folder 1/Folder 2/Disc 3"),
			};

			target.AvailableDiscs.Should().BeEquivalentTo(expectedAvailableDiscs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task LoadAvailableDiscsForAdviseSet_ForDiscsWithAdviseSetAssigned_DoesNotAddSuchDiscsToList()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1" },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3" },
			};

			var folder1 = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			var folder2 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			var folder3 = new FolderModel { Id = new ItemId("2"), Name = "Folder 2" };
			folder1.AddSubfolders(folder2);
			folder2.AddSubfolders(folder3);
			folder2.AddDiscs(discs[0]);
			folder1.AddDiscs(discs[1]);
			folder3.AddDiscs(discs[2]);

			var adviseSetDiscs = Array.Empty<DiscModel>();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);

			// Act

			target.LoadAvailableDiscsForAdviseSet(adviseSetDiscs);

			// Assert

			var expectedAvailableDiscs = new[]
			{
				new AvailableDiscViewModel(discs[0], "/Folder 1/Disc 1"),
				new AvailableDiscViewModel(discs[2], "/Folder 1/Folder 2/Disc 3"),
			};

			target.AvailableDiscs.Should().BeEquivalentTo(expectedAvailableDiscs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task LoadAvailableDiscsForAdviseSet_IfDiscBelongsToAnotherFolder_DoesNotAddSuchDiscsToList()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2" },
				new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3" },
			};

			var folder1 = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			var folder2 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			var folder3 = new FolderModel { Id = new ItemId("2"), Name = "Folder 2" };
			folder1.AddSubfolders(folder2);
			folder2.AddSubfolders(folder3);
			folder2.AddDiscs(discs[0], discs[2]);
			folder3.AddDiscs(discs[1]);

			var adviseSetDiscs = new[]
			{
				discs[0],
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);

			// Act

			target.LoadAvailableDiscsForAdviseSet(adviseSetDiscs);

			// Assert

			var expectedAvailableDiscs = new[]
			{
				new AvailableDiscViewModel(discs[2], "/Folder 1/Disc 3"),
			};

			target.AvailableDiscs.Should().BeEquivalentTo(expectedAvailableDiscs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task LoadAvailableDiscsForAdviseSet_IfDiscBelongsToAnotherAdviseGroup_DoesNotAddSuchDiscsToList()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("1") };
			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("2") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AdviseGroup = adviseGroup1, AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AdviseGroup = adviseGroup1 },
				new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3", AdviseGroup = adviseGroup2 },
				new DiscModel { Id = new ItemId("4"), TreeTitle = "Disc 4" },
			};

			var folder1 = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			var folder2 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			folder1.AddSubfolders(folder2);
			folder2.AddDiscs(discs);

			var adviseSetDiscs = new[]
			{
				discs[0],
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);

			// Act

			target.LoadAvailableDiscsForAdviseSet(adviseSetDiscs);

			// Assert

			var expectedAvailableDiscs = new[]
			{
				new AvailableDiscViewModel(discs[1], "/Folder 1/Disc 2"),
			};

			target.AvailableDiscs.Should().BeEquivalentTo(expectedAvailableDiscs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task SelectedDiscsCanBeAddedToAdviseSet_IfNoDiscsSelected_ReturnsFalse()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1" };
			var folder = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			folder.AddDiscs(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(new[] { disc }, CancellationToken.None);
			target.SelectedItems = new List<AvailableDiscViewModel>();

			// Act

			var result = target.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>());

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task SelectedDiscsCanBeAddedToAdviseSet_IfDiscsBelongsToDifferentFolders_ReturnsFalse()
		{
			// Arrange

			var adviseGroup1 = new AdviseGroupModel { Id = new ItemId("1") };
			var adviseGroup2 = new AdviseGroupModel { Id = new ItemId("2") };

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AdviseGroup = adviseGroup1 },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AdviseGroup = adviseGroup2 },
			};

			var folder = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			folder.AddDiscs(discs);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);
			target.SelectedItems = target.AvailableDiscs;

			// Act

			var result = target.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>());

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task SelectedDiscsCanBeAddedToAdviseSet_IfDiscsBelongsToDifferentAdviseGroups_ReturnsFalse()
		{
			// Arrange

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1" },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2" },
			};

			var folder1 = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			var folder2 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			folder1.AddSubfolders(folder2);
			folder1.AddDiscs(discs[0]);
			folder2.AddDiscs(discs[1]);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);
			target.SelectedItems = target.AvailableDiscs;

			// Act

			var result = target.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>());

			// Assert

			result.Should().BeFalse();
		}

		[TestMethod]
		public async Task SelectedDiscsCanBeAddedToAdviseSet_ForDiscsFromSameFolderAndAdviseGroup_ReturnsTrue()
		{
			// Arrange

			var discs = new[]
			{
				new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1" },
				new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2" },
			};

			var folder = new FolderModel { Id = new ItemId("0"), Name = "<ROOT" };
			folder.AddDiscs(discs);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AvailableDiscsViewModel>();

			await target.LoadDiscs(discs, CancellationToken.None);
			target.SelectedItems = target.AvailableDiscs;

			// Act

			var result = target.SelectedDiscsCanBeAddedToAdviseSet(Array.Empty<DiscModel>());

			// Assert

			result.Should().BeTrue();
		}
	}
}
