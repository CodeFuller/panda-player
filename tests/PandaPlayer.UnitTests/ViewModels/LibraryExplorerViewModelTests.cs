using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.LibraryExplorerEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class LibraryExplorerViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public async Task CreateAdviseGroup_IfShowCreateAdviseGroupViewReturnsNull_DoesNotCreateAdviseGroup()
		{
			// Arrange

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var viewNavigatorStub = new Mock<IViewNavigator>();
			viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns<string>(null);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperMock);
			mocker.Use(viewNavigatorStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.CreateAdviseGroup(Mock.Of<BasicAdviseGroupHolder>(), CancellationToken.None);

			// Assert

			adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(It.IsAny<BasicAdviseGroupHolder>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task CreateAdviseGroup_IfShowCreateAdviseGroupViewReturnsNotNull_CreatesAdviseGroup()
		{
			// Arrange

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var viewNavigatorStub = new Mock<IViewNavigator>();
			viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("Some New Group");

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperMock);
			mocker.Use(viewNavigatorStub);

			var adviseGroupHolder = Mock.Of<BasicAdviseGroupHolder>();

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.CreateAdviseGroup(adviseGroupHolder, CancellationToken.None);

			// Assert

			adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(adviseGroupHolder, "Some New Group", It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void PlayDisc_SendsPlaySongsListEventForActiveDiscSongs()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc = new DiscModel { Id = new ItemId("Some Disc"), Folder = discFolder };

			var activeSong1 = new SongModel { Id = new ItemId("Active Song 1"), Disc = disc };
			var activeSong2 = new SongModel { Id = new ItemId("Active Song 2"), Disc = disc };
			var deletedSong = new SongModel { Id = new ItemId("Deleted Songs"), Disc = disc, DeleteDate = new DateTime(2021, 07, 25) };

			disc.AllSongs = new[]
			{
				activeSong1,
				deletedSong,
				activeSong2,
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			PlaySongsListEventArgs playSongsListEvent = null;
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => e.RegisterEvent(ref playSongsListEvent));

			// Act

			target.PlayDisc(disc);

			// Assert

			var expectedSongs = new[]
			{
				activeSong1,
				activeSong2,
			};

			playSongsListEvent.Should().NotBeNull();
			playSongsListEvent.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void AddDiscToPlaylist_SendsAddingSongsToPlaylistLastEventWithActiveSongs()
		{
			// Arrange

			var activeSong1 = new SongModel { Id = new ItemId("Active Song 1") };
			var activeSong2 = new SongModel { Id = new ItemId("Active Song 2") };
			var deletedSong = new SongModel
			{
				Id = new ItemId("Deleted Songs"),
				DeleteDate = new DateTime(2021, 07, 25),
			};

			var disc = new DiscModel
			{
				Id = new ItemId("Some Disc"),
				AllSongs = new[]
				{
					activeSong1,
					deletedSong,
					activeSong2,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistLastEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistLastEvent));

			// Act

			target.AddDiscToPlaylist(disc);

			// Assert

			var expectedSongs = new[]
			{
				activeSong1,
				activeSong2,
			};

			addingSongsToPlaylistLastEvent.Should().NotBeNull();
			addingSongsToPlaylistLastEvent.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void EditDiscProperties_ShowDiscPropertiesView()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.EditDiscProperties(disc);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDiscPropertiesView(disc), Times.Once);
		}

		[TestMethod]
		public async Task DeleteFolder_IfFolderDeletionIsNotConfirmed_DoesNotDeleteFolder()
		{
			// Arrange

			var folder = new FolderModel { Id = new ItemId("Some Folder") };

			var mocker = new AutoMocker();

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.No);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteFolder(folder, CancellationToken.None);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteEmptyFolder(It.IsAny<FolderModel>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnFolderDeleted(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public async Task DeleteFolder_IfFolderHasSomeContent_ShowsWarningWithoutDeletingFolder()
		{
			// Arrange

			var folder = new FolderModel { Id = new ItemId("Some Folder") };
			folder.AddDiscs(new DiscModel { Id = new ItemId("Some Disc"), AllSongs = new[] { new SongModel { Id = new ItemId("Some Song") } } });

			var mocker = new AutoMocker();

			// We stub ShowMessageBoxResult.Yes just in case, to check that folder is not deleted due to content existence and not due to message result.
			var windowServiceMock = mocker.GetMock<IWindowService>();
			windowServiceMock
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteFolder(folder, CancellationToken.None);

			// Assert

			windowServiceMock.Verify(x => x.ShowMessageBox("You can not delete non-empty folder", "Warning", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation), Times.Once);

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteEmptyFolder(It.IsAny<FolderModel>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnFolderDeleted(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public async Task DeleteFolder_IfDeletionIsNotConfirmed_DoesNotDeleteFolder()
		{
			// Arrange

			var folder = new FolderModel { Id = new ItemId("Some Folder") };

			var mocker = new AutoMocker();

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.No);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteFolder(folder, CancellationToken.None);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteEmptyFolder(It.IsAny<FolderModel>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnFolderDeleted(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public async Task DeleteFolder_IfDeletionIsConfirmed_DeletesFolder()
		{
			// Arrange

			var folder = new FolderModel { Id = new ItemId("Some Folder") };

			var mocker = new AutoMocker();

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteFolder(folder, CancellationToken.None);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteEmptyFolder(folder, It.IsAny<CancellationToken>()), Times.Once);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnFolderDeleted(new ItemId("Some Folder")), Times.Once);
		}

		[TestMethod]
		public async Task DeleteDisc_InvokesShowDeleteDiscView()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteDisc(disc, CancellationToken.None);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDeleteDiscView(disc), Times.Once);
		}

		[TestMethod]
		public async Task DeleteDisc_IfDiscIsDeleted_InvokesOnDiscDeleted()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();

			mocker.GetMock<IViewNavigator>()
				.Setup(x => x.ShowDeleteDiscView(It.IsAny<DiscModel>())).Returns(true);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteDisc(disc, CancellationToken.None);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnDiscDeleted(new ItemId("Some Disc")), Times.Once);
		}

		[TestMethod]
		public async Task DeleteDisc_IfDiscIsNotDeleted_DoesNotInvokesOnDiscDeleted()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();

			mocker.GetMock<IViewNavigator>()
				.Setup(x => x.ShowDeleteDiscView(It.IsAny<DiscModel>())).Returns(false);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			await target.DeleteDisc(disc, CancellationToken.None);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.OnDiscDeleted(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void ApplicationLoadedEventArgs_LoadsAdviseGroupHelper()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			var adviseGroupHelperMock = mocker.GetMock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Verify(x => x.Load(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void LoadParentFolderEventHandler_LoadsParentFolderCorrectly()
		{
			// Arrange

			var parentFolder = new FolderModel { Id = new ItemId("Parent Folder Id") };
			var loadParentFolderEvent = new LoadParentFolderEventArgs(parentFolder, new ItemId("Child Folder Id"));

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(loadParentFolderEvent);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(parentFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectFolder(new ItemId("Child Folder Id")), Times.Once);
		}

		[TestMethod]
		public void LoadFolderEventHandler_LoadsFolderCorrectly()
		{
			// Arrange

			var folder = new FolderModel { Id = new ItemId("Some Folder Id") };
			var loadFolderEvent = new LoadFolderEventArgs(folder);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(loadFolderEvent);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(folder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectFolder(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void PlaySongsListEventHandler_AllSongsBelongToSingleDisc_SwitchesToThisDisc()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc = new DiscModel { Id = new ItemId("Some Disc") };
			discFolder.AddDiscs(disc);

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(new[] { song1, song2 }));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc")), Times.Once);
		}

		[TestMethod]
		public void PlaySongsListEventHandler_AllSongsBelongToSameAdviseSet_SwitchesToFirstDiscInAdviseSet()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("Some Advise Set") };
			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc1 = new DiscModel { Id = new ItemId("Some Disc 1"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) };
			var disc2 = new DiscModel { Id = new ItemId("Some Disc 2"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) };
			discFolder.AddDiscs(disc1);
			discFolder.AddDiscs(disc2);

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc1 };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc2 };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(new[] { song1, song2 }));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc 1")), Times.Once);
		}

		[TestMethod]
		public void PlaySongsListEventHandler_SongsBelongToDifferentDiscs_DoesNotSwitchToAnyDisc()
		{
			// Arrange

			var song1 = new SongModel
			{
				Id = new ItemId("1"),
				Disc = new DiscModel { Id = new ItemId("Disc 1") },
			};

			var song2 = new SongModel
			{
				Id = new ItemId("2"),
				Disc = new DiscModel { Id = new ItemId("Disc 2") },
			};

			var mocker = new AutoMocker();

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(new[] { song1, song2 }));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(It.IsAny<FolderModel>()), Times.Never);
			itemListViewModelMock.Verify(x => x.SelectDisc(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_AllSongsBelongToSingleDisc_SwitchesToThisDisc()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc = new DiscModel { Id = new ItemId("Some Disc") };
			discFolder.AddDiscs(disc);

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaylistLoadedEventArgs(new[] { song1, song2 }, song2, 1));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc")), Times.Once);
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_AllSongsBelongToSameAdviseSet_OpensDiscForCurrentPlaylistSong()
		{
			// Arrange

			var adviseSet = new AdviseSetModel { Id = new ItemId("Some Advise Set") };
			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc1 = new DiscModel { Id = new ItemId("Some Disc 1"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 1) };
			var disc2 = new DiscModel { Id = new ItemId("Some Disc 2"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 2) };
			discFolder.AddDiscs(disc1);
			discFolder.AddDiscs(disc2);

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc1 };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc2 };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaylistLoadedEventArgs(new[] { song1, song2 }, song2, 1));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc 2")), Times.Once);
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_SongsBelongToDifferentDiscs_LoadsRootFolder()
		{
			// Arrange

			var rootFolder = new FolderModel();

			var song1 = new SongModel { Id = new ItemId("1"), Disc = new DiscModel { Id = new ItemId("Disc 1") } };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = new DiscModel { Id = new ItemId("Disc 2") } };

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetRootFolder(It.IsAny<CancellationToken>())).ReturnsAsync(rootFolder);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new PlaylistLoadedEventArgs(new[] { song1, song2 }, song2, 1));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(rootFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void NoPlaylistLoadedEventHandler_LoadsRootFolder()
		{
			// Arrange

			var rootFolder = new FolderModel();

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetRootFolder(It.IsAny<CancellationToken>())).ReturnsAsync(rootFolder);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new NoPlaylistLoadedEventArgs());

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.LoadFolderItems(rootFolder), Times.Once);
		}

		[TestMethod]
		public void NavigateLibraryExplorerToDiscEvent_SwitchesToThisDisc()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc = new DiscModel { Id = new ItemId("Some Disc") };
			discFolder.AddDiscs(disc);

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(disc));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc")), Times.Once);
		}
	}
}
