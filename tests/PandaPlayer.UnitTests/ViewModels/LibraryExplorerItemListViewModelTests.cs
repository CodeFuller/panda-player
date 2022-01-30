using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.LibraryExplorerItems;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class LibraryExplorerItemListViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void DiscsGetter_ReturnsActiveDiscs()
		{
			// Arrange

			var activeDisc1 = new DiscModel
			{
				TreeTitle = "Disc 1",
				AllSongs = new[] { new SongModel() },
			};

			var activeDisc2 = new DiscModel
			{
				TreeTitle = "Disc 2",
				AllSongs = new[] { new SongModel() },
			};

			var deletedDisc = new DiscModel
			{
				TreeTitle = "Deleted Disc",
				AllSongs = new[]
				{
					new SongModel { DeleteDate = new DateTime(2021, 07, 25) },
				},
			};

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel(),
				},

				Discs = new[]
				{
					activeDisc1,
					deletedDisc,
					activeDisc2,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			// Act

			var discs = target.Discs;

			// Assert

			var expectedDiscs = new[]
			{
				activeDisc1,
				activeDisc2,
			};

			discs.Should().BeEquivalentTo(expectedDiscs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void SelectedFolder_IfFolderIsSelected_ReturnsThisFolder()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Folder 1" },
					new FolderModel { Name = "Folder 2" },
					new FolderModel { Name = "Folder 3" },
				},

				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = target.Items[1];
			target.SelectedItem.Should().BeOfType<FolderExplorerItem>();

			// Act

			var selectedFolder = target.SelectedFolder;

			// Assert

			selectedFolder.Should().Be(folder.Subfolders.ToList()[1]);
		}

		[TestMethod]
		public void SelectedFolder_IfDiscIsSelected_ReturnsNull()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Some Folder" },
				},

				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = target.Items[1];
			target.SelectedItem.Should().BeOfType<DiscExplorerItem>();

			// Act

			var selectedFolder = target.SelectedFolder;

			// Assert

			selectedFolder.Should().BeNull();
		}

		[TestMethod]
		public void SelectedFolder_IfNoItemIsSelected_ReturnsNull()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Some Folder" },
				},

				Discs = Array.Empty<DiscModel>(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = null;

			// Act

			var selectedFolder = target.SelectedFolder;

			// Assert

			selectedFolder.Should().BeNull();
		}

		[TestMethod]
		public void SelectedDisc_IfDiscIsSelected_ReturnsThisDisc()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Some Folder" },
				},

				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Disc 1",
						AllSongs = new[] { new SongModel() },
					},
					new DiscModel
					{
						TreeTitle = "Disc 2",
						AllSongs = new[] { new SongModel() },
					},
					new DiscModel
					{
						TreeTitle = "Disc 3",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = target.Items[2];
			target.SelectedItem.Should().BeOfType<DiscExplorerItem>();

			// Act

			var selectedDisc = target.SelectedDisc;

			// Assert

			selectedDisc.Should().Be(folder.Discs.ToList()[1]);
		}

		[TestMethod]
		public void SelectedDisc_IfFolderIsSelected_ReturnsNull()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Name = "Some Folder" },
				},

				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = target.Items[0];
			target.SelectedItem.Should().BeOfType<FolderExplorerItem>();

			// Act

			var selectedDisc = target.SelectedDisc;

			// Assert

			selectedDisc.Should().BeNull();
		}

		[TestMethod]
		public void SelectedDisc_IfNoItemIsSelected_ReturnsNull()
		{
			// Arrange

			var folder = new FolderModel
			{
				Subfolders = Array.Empty<FolderModel>(),
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			target.SelectedItem = null;

			// Act

			var selectedDisc = target.SelectedDisc;

			// Assert

			selectedDisc.Should().BeNull();
		}

		[TestMethod]
		public void ShowDeletedContentSetter_IfFolderWasSelectedBefore_SelectsSameFolder()
		{
			// Arrange

			var subfolder1 = new FolderModel { Id = new ItemId("1"), Name = "Subfolder 1" };
			var subfolder2 = new FolderModel { Id = new ItemId("2"), Name = "Subfolder 2" };
			var subfolder3 = new FolderModel { Id = new ItemId("3"), Name = "Subfolder 3" };

			var disc1 = new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AllSongs = new[] { new SongModel() } };
			var disc3 = new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder1, subfolder2, subfolder3 },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectFolder(new ItemId("2"));

			// Act

			target.ShowDeletedContent = true;

			// Assert

			target.SelectedFolder.Should().Be(subfolder2);
		}

		[TestMethod]
		public void ShowDeletedContentSetter_IfDiscWasSelectedBefore_SelectsSameDisc()
		{
			// Arrange

			var subfolder1 = new FolderModel { Id = new ItemId("1"), Name = "Subfolder 1" };
			var subfolder2 = new FolderModel { Id = new ItemId("2"), Name = "Subfolder 2" };
			var subfolder3 = new FolderModel { Id = new ItemId("3"), Name = "Subfolder 3" };

			var disc1 = new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AllSongs = new[] { new SongModel() } };
			var disc3 = new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder1, subfolder2, subfolder3 },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectDisc(new ItemId("2"));

			// Act

			target.ShowDeletedContent = true;

			// Assert

			target.SelectedDisc.Should().Be(disc2);
		}

		[TestMethod]
		public void ShowDeletedContentSetter_IfPreviouslySelectedItemIsMissing_SelectsFirstItem()
		{
			// Arrange

			var subfolder1 = new FolderModel { Id = new ItemId("1"), Name = "Subfolder 1" };
			var subfolder2 = new FolderModel { Id = new ItemId("2"), Name = "Subfolder 2", DeleteDate = new DateTime(2021, 07, 25) };
			var subfolder3 = new FolderModel { Id = new ItemId("3"), Name = "Subfolder 3" };

			var disc1 = new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AllSongs = new[] { new SongModel { DeleteDate = new DateTime(2021, 07, 25) } } };
			var disc3 = new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder1, subfolder2, subfolder3 },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = true;
			target.SelectDisc(new ItemId("2"));

			// Act

			target.ShowDeletedContent = false;

			// Assert

			target.SelectedFolder.Should().Be(subfolder1);
		}

		[TestMethod]
		public void LoadFolderItems_ForNonRootFolder_AddsParentFolderExplorerItem()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder") },
				Subfolders = Array.Empty<FolderModel>(),
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);

			// Assert

			var firstItem = target.Items.First();
			firstItem.Should().BeOfType<ParentFolderExplorerItem>();
			target.SelectedItem.Should().Be(firstItem);
		}

		[TestMethod]
		public void LoadFolderItems_ForRootFolder_DoesNotAddParentFolderExplorerItem()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = null,
				Subfolders = Array.Empty<FolderModel>(),
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);

			// Assert

			target.Items.OfType<ParentFolderExplorerItem>().Should().BeEmpty();
		}

		[TestMethod]
		public void LoadFolderItems_ForFolderWithSubfoldersAndDiscs_OrdersItemsCorrectly()
		{
			// Arrange

			var folder1 = new FolderModel { Name = "21 - Folder 1" };
			var folder2 = new FolderModel { Name = "22 - Folder 2" };
			var folder3 = new FolderModel { Name = "23 - Folder 3" };

			var disc1 = new DiscModel
			{
				TreeTitle = "11 - Disc 1",
				AllSongs = new[] { new SongModel() },
			};

			var disc2 = new DiscModel
			{
				TreeTitle = "12 - Disc 2",
				AllSongs = new[] { new SongModel() },
			};

			var disc3 = new DiscModel
			{
				TreeTitle = "13 - Disc 3",
				AllSongs = new[] { new SongModel() },
			};

			var folder = new FolderModel
			{
				Subfolders = new[] { folder3, folder1, folder2 },
				Discs = new[] { disc3, disc1, disc2, },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(folder1),
				new FolderExplorerItem(folder2),
				new FolderExplorerItem(folder3),

				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc2),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
			target.SelectedItem.Should().Be(target.Items.First());
		}

		[TestMethod]
		public void LoadFolderItems_ForTitlesInDifferentCase_IgnoresCaseDuringOrdering()
		{
			// Arrange

			var folder1 = new FolderModel { Name = "A Folder" };
			var folder2 = new FolderModel { Name = "b Folder" };
			var folder3 = new FolderModel { Name = "C Folder" };

			var disc1 = new DiscModel
			{
				TreeTitle = "A Disc",
				AllSongs = new[] { new SongModel() },
			};

			var disc2 = new DiscModel
			{
				TreeTitle = "b Disc",
				AllSongs = new[] { new SongModel() },
			};

			var disc3 = new DiscModel
			{
				TreeTitle = "C Disc",
				AllSongs = new[] { new SongModel() },
			};

			var folder = new FolderModel
			{
				Subfolders = new[] { folder3, folder1, folder2 },
				Discs = new[] { disc3, disc1, disc2, },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(folder1),
				new FolderExplorerItem(folder2),
				new FolderExplorerItem(folder3),

				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc2),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
			target.SelectedItem.Should().Be(target.Items.First());
		}

		// Previously we have used StringComparer.OrdinalIgnoreCase for ordering items.
		// This comparer puts titles starting from character 'Ё' before all other letters in Russian language.
		[TestMethod]
		public void LoadFolderItems_IfTitlesContainSpecialLocalCharacters_OrdersItemsCorrectly()
		{
			// Arrange

			var folder1 = new FolderModel { Name = "Елена Никитаева" };
			var folder2 = new FolderModel { Name = "Ёлка" };
			var folder3 = new FolderModel { Name = "Жанна Агузарова" };

			var disc1 = new DiscModel
			{
				TreeTitle = "Елена Никитаева",
				AllSongs = new[] { new SongModel() },
			};

			var disc2 = new DiscModel
			{
				TreeTitle = "Ёлка",
				AllSongs = new[] { new SongModel() },
			};

			var disc3 = new DiscModel
			{
				TreeTitle = "Жанна Агузарова",
				AllSongs = new[] { new SongModel() },
			};

			var folder = new FolderModel
			{
				Subfolders = new[] { folder3, folder1, folder2 },
				Discs = new[] { disc3, disc1, disc2, },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(folder1),
				new FolderExplorerItem(folder2),
				new FolderExplorerItem(folder3),

				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc2),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
			target.SelectedItem.Should().Be(target.Items.First());
		}

		[TestMethod]
		public void LoadFolderItems_ShowDeletedContentIsFalse_DoesNotLoadDeletedContent()
		{
			// Arrange

			var subfolder1 = new FolderModel { Name = "Subfolder 1" };
			var subfolder2 = new FolderModel { Name = "Subfolder 2", DeleteDate = new DateTime(2021, 07, 25) };
			var subfolder3 = new FolderModel { Name = "Subfolder 3" };

			var disc1 = new DiscModel { TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { TreeTitle = "Disc 2", AllSongs = new[] { new SongModel { DeleteDate = new DateTime(2021, 07, 25) } } };
			var disc3 = new DiscModel { TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder1, subfolder2, subfolder3 },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();
			target.ShowDeletedContent = false;

			// Act

			target.LoadFolderItems(folder);

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(subfolder1),
				new FolderExplorerItem(subfolder3),
				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void LoadFolderItems_ShowDeletedContentIsTrue_LoadsDeletedContent()
		{
			// Arrange

			var subfolder1 = new FolderModel { Name = "Subfolder 1" };
			var subfolder2 = new FolderModel { Name = "Subfolder 2", DeleteDate = new DateTime(2021, 07, 25) };
			var subfolder3 = new FolderModel { Name = "Subfolder 3" };

			var disc1 = new DiscModel { TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { TreeTitle = "Disc 2", AllSongs = new[] { new SongModel { DeleteDate = new DateTime(2021, 07, 25) } } };
			var disc3 = new DiscModel { TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder1, subfolder2, subfolder3 },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			// Act

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = true;

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(subfolder1),
				new FolderExplorerItem(subfolder2),
				new FolderExplorerItem(subfolder3),
				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc2),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void LoadFolderItems_IfSomeItemsWereLoadedBefore_ClearsPreviousItems()
		{
			// Arrange

			var oldFolder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel { Id = new ItemId("OldSubfolder"), Name = "Old Folder" },
				},

				Discs = new[]
				{
					new DiscModel { Id = new ItemId("OldDisc"), TreeTitle = "Old Disc", AllSongs = new[] { new SongModel { Id = new ItemId("1") } } },
				},
			};

			var newSubfolder = new FolderModel { Id = new ItemId("NewSubfolder"), Name = "New Folder" };
			var newDisc = new DiscModel { Id = new ItemId("NewSubfolder"), TreeTitle = "New Disc", AllSongs = new[] { new SongModel { Id = new ItemId("2") } } };
			var newFolder = new FolderModel { Id = new ItemId("NewFolder"), Subfolders = new[] { newSubfolder }, Discs = new[] { newDisc } };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(oldFolder);
			target.SelectedItem = target.Items[1];

			// Act

			target.LoadFolderItems(newFolder);

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(newSubfolder),
				new DiscExplorerItem(newDisc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
			target.SelectedItem.Should().Be(target.Items.First());
		}

		[TestMethod]
		public void SelectFolder_IfFolderPresentsInList_SelectsThisFolder()
		{
			// Arrange

			var folder1 = new FolderModel
			{
				Id = new ItemId("1"),
				Name = "Folder 1",
			};

			var folder2 = new FolderModel
			{
				Id = new ItemId("2"),
				Name = "Folder 2",
			};

			var folder3 = new FolderModel
			{
				Id = new ItemId("3"),
				Name = "Folder 3",
			};

			var folder = new FolderModel
			{
				Subfolders = new[] { folder1, folder2, folder3 },
				Discs = new[]
				{
					new DiscModel
					{
						// Using the same id as for requested folder, just in case.
						Id = new ItemId("2"),
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			// Act

			target.SelectFolder(new ItemId("2"));

			// Assert

			target.SelectedItem.Should().BeOfType<FolderExplorerItem>();
			target.SelectedItem.Title.Should().Be("Folder 2");
		}

		[TestMethod]
		public void SelectFolder_IfFolderDoesNotPresentInList_ClearsSelection()
		{
			// Arrange

			var subfolder = new FolderModel
			{
				Id = new ItemId("Subfolder Id"),
				Name = "Some Subfolder",
			};

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder },
				Discs = new[]
				{
					new DiscModel
					{
						// Using the same id as for requested folder, just in case.
						Id = new ItemId("1"),
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem.Should().NotBeNull();

			// Act

			target.SelectFolder(new ItemId("1"));

			// Assert

			target.SelectedItem.Should().BeNull();
		}

		[TestMethod]
		public void SelectDisc_IfDiscPresentsInList_SelectsThisDisc()
		{
			// Arrange

			var disc1 = new DiscModel
			{
				Id = new ItemId("1"),
				TreeTitle = "Disc 1",
				AllSongs = new[] { new SongModel() },
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("2"),
				TreeTitle = "Disc 2",
				AllSongs = new[] { new SongModel() },
			};

			var disc3 = new DiscModel
			{
				Id = new ItemId("3"),
				TreeTitle = "Disc 3",
				AllSongs = new[] { new SongModel() },
			};

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel
					{
						// Using the same id as for requested disc, just in case.
						Id = new ItemId("2"),
						Name = "Some Subfolder",
					},
				},
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			// Act

			target.SelectDisc(new ItemId("2"));

			// Assert

			target.SelectedItem.Should().BeOfType<DiscExplorerItem>();
			target.SelectedItem.Title.Should().Be("Disc 2");
		}

		[TestMethod]
		public void SelectDisc_IfDiscDoesNotPresentInList_ClearsSelection()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("Disc Id"),
				TreeTitle = "Some Disc",
				AllSongs = new[] { new SongModel() },
			};

			var folder = new FolderModel
			{
				Subfolders = new[]
				{
					new FolderModel
					{
						// Using the same id as for requested disc, just in case.
						Id = new ItemId("1"),
						Name = "Some Subfolder",
					},
				},
				Discs = new[] { disc },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			// Act

			target.SelectDisc(new ItemId("1"));

			// Assert

			target.SelectedItem.Should().BeNull();
		}

		[TestMethod]
		public async Task OnFolderDeleted_IfDeletedContentIsHiddenAndFolderPresentsInList_RemovesThisFolder()
		{
			// Arrange

			var folder1 = new FolderModel { Id = new ItemId("1"), Name = "Folder 1" };
			var folder2 = new FolderModel { Id = new ItemId("2"), Name = "Folder 2" };
			var folder3 = new FolderModel { Id = new ItemId("3"), Name = "Folder 3" };

			// Using the same id as for requested folder, just in case.
			var disc = new DiscModel { Id = new ItemId("2"), TreeTitle = "Some Disc", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { folder1, folder2, folder3 },
				Discs = new[] { disc },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = false;

			// Act

			await target.OnFolderDeleted(new ItemId("2"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(folder1),
				new FolderExplorerItem(folder3),
				new DiscExplorerItem(disc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task OnFolderDeleted_IfDeletedContentIsHiddenAndFolderDoesNotPresentInList_DoesNothing()
		{
			// Arrange

			var subfolder = new FolderModel { Id = new ItemId("Subfolder Id"), Name = "Some Subfolder" };
			var disc = new DiscModel { Id = new ItemId("1"), TreeTitle = "Some Disc", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder },
				Discs = new[] { disc },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = false;

			// Act

			await target.OnFolderDeleted(new ItemId("1"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(subfolder),
				new DiscExplorerItem(disc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task OnFolderDeleted_IfDeletedContentIsShown_ReloadsCurrentFolder()
		{
			// Arrange

			var oldSubfolder = new FolderModel { Id = new ItemId("Old Subfolder Id"), Name = "Old Subfolder" };

			var folder = new FolderModel
			{
				Subfolders = new[] { oldSubfolder },
				Discs = Array.Empty<DiscModel>(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = true;

			// We add new items to the current folder for checking that LoadFolderItems() was called once again for the current folder.
			var newSubfolder = new FolderModel { Id = new ItemId("New Subfolder Id"), Name = "New Subfolder" };
			var newDisc = new DiscModel { Id = new ItemId("New Disc Id"), TreeTitle = "Some Disc", AllSongs = new[] { new SongModel() } };
			folder.AddSubfolder(newSubfolder);
			folder.AddDisc(newDisc);

			// Act

			await target.OnFolderDeleted(new ItemId("Old Subfolder Id"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(newSubfolder),
				new FolderExplorerItem(oldSubfolder),
				new DiscExplorerItem(newDisc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task OnDiscDeleted_IfDeletedContentIsHiddenAndDiscPresentsInList_RemovesThisDisc()
		{
			// Arrange

			// Using the same id as for requested disc, just in case.
			var subfolder = new FolderModel { Id = new ItemId("2"), Name = "Some Subfolder" };

			var disc1 = new DiscModel { Id = new ItemId("1"), TreeTitle = "Disc 1", AllSongs = new[] { new SongModel() } };
			var disc2 = new DiscModel { Id = new ItemId("2"), TreeTitle = "Disc 2", AllSongs = new[] { new SongModel() } };
			var disc3 = new DiscModel { Id = new ItemId("3"), TreeTitle = "Disc 3", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel
			{
				Subfolders = new[] { subfolder },
				Discs = new[] { disc1, disc2, disc3 },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = false;

			// Act

			await target.OnDiscDeleted(new ItemId("2"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(subfolder),
				new DiscExplorerItem(disc1),
				new DiscExplorerItem(disc3),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task OnDiscDeleted_IfDeletedContentIsHiddenAndDiscDoesNotPresentInList_DoesNothing()
		{
			// Arrange

			// Using the same id as for requested disc, just in case.
			var subfolder = new FolderModel { Id = new ItemId("1"), Name = "Some Subfolder" };
			var disc = new DiscModel { Id = new ItemId("Disc Id"), TreeTitle = "Some Disc", AllSongs = new[] { new SongModel() } };

			var folder = new FolderModel { Subfolders = new[] { subfolder }, Discs = new[] { disc } };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = false;

			// Act

			await target.OnDiscDeleted(new ItemId("1"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(subfolder),
				new DiscExplorerItem(disc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task OnDiscDeleted_IfDeletedContentIsShown_ReloadsCurrentFolder()
		{
			// Arrange

			var oldDisc = new DiscModel { Id = new ItemId("Old Disc Id"), TreeTitle = "Old Disc", AllSongs = new[] { new SongModel { Id = new ItemId("1") } } };

			var folder = new FolderModel
			{
				Subfolders = Array.Empty<FolderModel>(),
				Discs = new[] { oldDisc },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.ShowDeletedContent = true;

			// We add new items to the current folder for checking that LoadFolderItems() was called once again for the current folder.
			var newSubfolder = new FolderModel { Id = new ItemId("New Subfolder Id"), Name = "New Subfolder" };
			var newDisc = new DiscModel { Id = new ItemId("New Disc Id"), TreeTitle = "New Disc", AllSongs = new[] { new SongModel { Id = new ItemId("2") } } };
			folder.AddSubfolder(newSubfolder);
			folder.AddDisc(newDisc);

			// Act

			await target.OnDiscDeleted(new ItemId("Old Disc Id"));

			// Assert

			var expectedItems = new BasicExplorerItem[]
			{
				new FolderExplorerItem(newSubfolder),
				new DiscExplorerItem(newDisc),
				new DiscExplorerItem(oldDisc),
			};

			target.Items.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void ChangeFolderCommandHandler_IfParentFolderItemIsSelected_SendsLoadParentFolderEvent()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = Array.Empty<FolderModel>(),
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem.Should().BeOfType<ParentFolderExplorerItem>();

			LoadParentFolderEventArgs loadParentFolderEvent = null;
			Messenger.Default.Register<LoadParentFolderEventArgs>(this, e => e.RegisterEvent(ref loadParentFolderEvent));

			// Act

			target.ChangeFolderCommand.Execute(null);

			// Assert

			loadParentFolderEvent.Should().NotBeNull();
			loadParentFolderEvent.ParentFolder.Should().Be(folder.ParentFolder);
			loadParentFolderEvent.ChildFolderId.Should().Be(new ItemId("Child Folder Id"));
		}

		[TestMethod]
		public void ChangeFolderCommandHandler_IfNormalFolderItemIsSelected_SendsLoadFolderEvent()
		{
			// Arrange

			var subfolder = new FolderModel { Id = new ItemId("Subfolder Id") };

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = new[] { subfolder },
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem = target.Items[1];
			target.SelectedItem.Should().BeOfType<FolderExplorerItem>();

			LoadFolderEventArgs loadFolderEvent = null;
			Messenger.Default.Register<LoadFolderEventArgs>(this, e => e.RegisterEvent(ref loadFolderEvent));

			// Act

			target.ChangeFolderCommand.Execute(null);

			// Assert

			loadFolderEvent.Should().NotBeNull();
			loadFolderEvent.Folder.Should().Be(subfolder);
		}

		[TestMethod]
		public void ChangeFolderCommandHandler_IfDiscItemIsSelected_DoesNothing()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = new[]
				{
					new FolderModel { Id = new ItemId("Subfolder Id") },
				},
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem = target.Items[2];
			target.SelectedItem.Should().BeOfType<DiscExplorerItem>();

			LoadParentFolderEventArgs loadParentFolderEvent = null;
			Messenger.Default.Register<LoadParentFolderEventArgs>(this, e => e.RegisterEvent(ref loadParentFolderEvent));

			LoadFolderEventArgs loadFolderEvent = null;
			Messenger.Default.Register<LoadFolderEventArgs>(this, e => e.RegisterEvent(ref loadFolderEvent));

			// Act

			target.ChangeFolderCommand.Execute(null);

			// Assert

			loadParentFolderEvent.Should().BeNull();
			loadFolderEvent.Should().BeNull();
		}

		[TestMethod]
		public void ChangeFolderCommandHandler_IfNoItemIsSelected_DoesNothing()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = new[]
				{
					new FolderModel { Id = new ItemId("Subfolder Id") },
				},
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem = null;

			LoadParentFolderEventArgs loadParentFolderEvent = null;
			Messenger.Default.Register<LoadParentFolderEventArgs>(this, e => e.RegisterEvent(ref loadParentFolderEvent));

			LoadFolderEventArgs loadFolderEvent = null;
			Messenger.Default.Register<LoadFolderEventArgs>(this, e => e.RegisterEvent(ref loadFolderEvent));

			// Act

			target.ChangeFolderCommand.Execute(null);

			// Assert

			loadParentFolderEvent.Should().BeNull();
			loadFolderEvent.Should().BeNull();
		}

		[TestMethod]
		public void JumpToFirstItemCommandHandler_SelectsFirstListItem()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = new[]
				{
					new FolderModel { Id = new ItemId("Subfolder Id") },
				},
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);
			target.SelectedItem = target.Items.Last();

			// Act

			target.JumpToFirstItemCommand.Execute(null);

			// Assert

			target.SelectedItem.Should().Be(target.Items.First());
		}

		[TestMethod]
		public void JumpToLastItemCommandHandler_SelectsLastListItem()
		{
			// Arrange

			var folder = new FolderModel
			{
				ParentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") },
				Id = new ItemId("Child Folder Id"),
				Subfolders = new[]
				{
					new FolderModel { Id = new ItemId("Subfolder Id") },
				},
				Discs = new[]
				{
					new DiscModel
					{
						TreeTitle = "Some Disc",
						AllSongs = new[] { new SongModel() },
					},
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerItemListViewModel>();

			target.LoadFolderItems(folder);

			// Act

			target.JumpToLastItemCommand.Execute(null);

			// Assert

			target.SelectedItem.Should().Be(target.Items.Last());
		}
	}
}
