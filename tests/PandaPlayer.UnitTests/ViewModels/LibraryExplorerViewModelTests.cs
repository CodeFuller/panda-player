using System;
using System.Collections.Generic;
using System.Linq;
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
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.AdviseGroups;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

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
		public void PlayDiscCommand_IfSomeDiscIsSelected_SendsPlaySongsListEventWithSelectedDisc()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var selectedDisc = new DiscModel { Id = new ItemId("Some Disc"), Folder = discFolder };

			var activeSong1 = new SongModel { Id = new ItemId("Active Song 1"), Disc = selectedDisc };
			var activeSong2 = new SongModel { Id = new ItemId("Active Song 2"), Disc = selectedDisc };
			var deletedSong = new SongModel { Id = new ItemId("Deleted Songs"), Disc = selectedDisc, DeleteDate = new DateTime(2021, 07, 25) };

			selectedDisc.AllSongs = new[]
			{
				activeSong1,
				deletedSong,
				activeSong2,
			};

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			PlaySongsListEventArgs playSongsListEvent = null;
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => e.RegisterEvent(ref playSongsListEvent));

			// Act

			target.PlayDiscCommand.Execute(null);

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
		public void PlayDiscCommand_IfNoDiscIsSelected_DoesNotSendPlaySongsListEvent()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns<DiscModel>(null);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			PlaySongsListEventArgs playSongsListEvent = null;
			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => e.RegisterEvent(ref playSongsListEvent));

			// Act

			target.PlayDiscCommand.Execute(null);

			// Assert

			playSongsListEvent.Should().BeNull();
		}

		[TestMethod]
		public void AddDiscToPlaylistCommand_IfSomeDiscIsSelected_SendsAddingSongsToPlaylistLastEventWithActiveSongs()
		{
			// Arrange

			var activeSong1 = new SongModel { Id = new ItemId("Active Song 1") };
			var activeSong2 = new SongModel { Id = new ItemId("Active Song 2") };
			var deletedSong = new SongModel
			{
				Id = new ItemId("Deleted Songs"),
				DeleteDate = new DateTime(2021, 07, 25),
			};

			var selectedDisc = new DiscModel
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
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistLastEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistLastEvent));

			// Act

			target.AddDiscToPlaylistCommand.Execute(null);

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
		public void AddDiscToPlaylistCommand_IfNoDiscIsSelected_DoesNotSendAddingSongsToPlaylistLastEvent()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns<DiscModel>(null);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistLastEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistLastEvent));

			// Act

			target.AddDiscToPlaylistCommand.Execute(null);

			// Assert

			addingSongsToPlaylistLastEvent.Should().BeNull();
		}

		[TestMethod]
		public void EditDiscPropertiesCommand_IfSomeDiscIsSelected_ShowDiscPropertiesViewForSelectedDisc()
		{
			// Arrange

			var selectedDisc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.EditDiscPropertiesCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDiscPropertiesView(selectedDisc), Times.Once);
		}

		[TestMethod]
		public void EditDiscPropertiesCommand_IfNoDiscIsSelected_DoesNotShowDiscPropertiesView()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns<DiscModel>(null);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.EditDiscPropertiesCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDiscPropertiesView(It.IsAny<DiscModel>()), Times.Never);
		}

		[TestMethod]
		public void DeleteFolderCommand_IfNoFolderIsSelected_DoesNothing()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedFolder).Returns<FolderModel>(null);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteFolderCommand.Execute(null);

			// Assert

			var windowServiceMock = mocker.GetMock<IWindowService>();
			windowServiceMock.Verify(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>()), Times.Never);

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteFolder(It.IsAny<ItemId>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveFolder(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void DeleteFolderCommand_IfFolderDeletionIsNotConfirmed_DoesNotDeleteFolder()
		{
			// Arrange

			var selectedFolder = new FolderModel
			{
				Id = new ItemId("Some Folder"),
				Subfolders = Array.Empty<ShallowFolderModel>(),
				Discs = Array.Empty<DiscModel>(),
			};

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedFolder).Returns(selectedFolder);

			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(selectedFolder);

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.No);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteFolderCommand.Execute(null);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteFolder(It.IsAny<ItemId>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveFolder(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void DeleteFolderCommand_IfSelectedFolderHasSomeContent_ShowsWarningWithoutDeletingFolder()
		{
			// Arrange

			var selectedFolder = new FolderModel
			{
				Id = new ItemId("Some Folder"),
				Subfolders = Array.Empty<ShallowFolderModel>(),
				Discs = new[]
				{
					new DiscModel
					{
						Id = new ItemId("Some Disc"),
						AllSongs = new[]
						{
							new SongModel { Id = new ItemId("Some Song") },
						},
					},
				},
			};

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedFolder).Returns(selectedFolder);

			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(selectedFolder);

			// We stub ShowMessageBoxResult.Yes just in case, to check that folder is not deleted due to content existence and not due to message result.
			var windowServiceMock = mocker.GetMock<IWindowService>();
			windowServiceMock
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteFolderCommand.Execute(null);

			// Assert

			windowServiceMock.Verify(x => x.ShowMessageBox("You can not delete non-empty folder", "Warning", ShowMessageBoxButton.Ok, ShowMessageBoxIcon.Exclamation), Times.Once);

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteFolder(It.IsAny<ItemId>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveFolder(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void DeleteFolderCommand_IfDeletionIsNotConfirmed_DoesNotDeleteFolder()
		{
			// Arrange

			var selectedFolder = new FolderModel
			{
				Id = new ItemId("Some Folder"),
				Subfolders = Array.Empty<ShallowFolderModel>(),
				Discs = Array.Empty<DiscModel>(),
			};

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedFolder).Returns(selectedFolder);

			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(selectedFolder);

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.No);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteFolderCommand.Execute(null);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteFolder(It.IsAny<ItemId>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveFolder(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void DeleteFolderCommand_IfDeletionIsConfirmed_DeletesSelectedFolder()
		{
			// Arrange

			var selectedFolder = new FolderModel
			{
				Id = new ItemId("Some Folder"),
				Subfolders = Array.Empty<ShallowFolderModel>(),
				Discs = Array.Empty<DiscModel>(),
			};

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedFolder).Returns(selectedFolder);

			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(selectedFolder);

			mocker.GetMock<IWindowService>()
				.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>())).Returns(ShowMessageBoxResult.Yes);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteFolderCommand.Execute(null);

			// Assert

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.DeleteFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>()), Times.Once);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveFolder(new ItemId("Some Folder")), Times.Once);
		}

		[TestMethod]
		public void DeleteDiscCommand_IfNoDiscIsSelected_DoesNothing()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns<DiscModel>(null);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteDiscCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDeleteDiscView(It.IsAny<DiscModel>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveDisc(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void DeleteDiscCommand_IfSomeDiscIsSelected_InvokesShowDeleteDiscViewForSelectedDisc()
		{
			// Arrange

			var selectedDisc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteDiscCommand.Execute(null);

			// Assert

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowDeleteDiscView(selectedDisc), Times.Once);
		}

		[TestMethod]
		public void DeleteDiscCommand_IfDiscIsDeleted_RemovesDiscFromItemList()
		{
			// Arrange

			var selectedDisc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			mocker.GetMock<IViewNavigator>()
				.Setup(x => x.ShowDeleteDiscView(It.IsAny<DiscModel>())).Returns(true);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteDiscCommand.Execute(null);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveDisc(new ItemId("Some Disc")), Times.Once);
		}

		[TestMethod]
		public void DeleteDiscCommand_IfDiscIsNotDeleted_DoesNotRemoveDiscFromItemList()
		{
			// Arrange

			var selectedDisc = new DiscModel { Id = new ItemId("Some Disc") };

			var mocker = new AutoMocker();

			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			mocker.GetMock<IViewNavigator>()
				.Setup(x => x.ShowDeleteDiscView(It.IsAny<DiscModel>())).Returns(false);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			target.DeleteDiscCommand.Execute(null);

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();
			itemListViewModelMock.Verify(x => x.RemoveDisc(It.IsAny<ItemId>()), Times.Never);
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

			var parentFolder = new FolderModel();

			var loadParentFolderEvent = new LoadParentFolderEventArgs(new ItemId("Parent Folder Id"), new ItemId("Child Folder Id"));

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Parent Folder Id"), It.IsAny<CancellationToken>())).ReturnsAsync(parentFolder);

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

			var folder = new FolderModel();

			var loadFolderEvent = new LoadFolderEventArgs(new ItemId("Some Folder Id"));

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder Id"), It.IsAny<CancellationToken>())).ReturnsAsync(folder);

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
			var disc = new DiscModel { Id = new ItemId("Some Disc"), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc };

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(discFolder);

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
			var disc1 = new DiscModel { Id = new ItemId("Some Disc 1"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 1), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };
			var disc2 = new DiscModel { Id = new ItemId("Some Disc 2"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 2), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc1 };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc2 };

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(discFolder);

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

			var folderServiceMock = mocker.GetMock<IFoldersService>();
			folderServiceMock.Verify(x => x.GetFolder(It.IsAny<ItemId>(), It.IsAny<CancellationToken>()), Times.Never);

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(It.IsAny<FolderModel>()), Times.Never);
			itemListViewModelMock.Verify(x => x.SelectDisc(It.IsAny<ItemId>()), Times.Never);
		}

		[TestMethod]
		public void PlaylistLoadedEventHandler_AllSongsBelongToSingleDisc_SwitchesToThisDisc()
		{
			// Arrange

			var discFolder = new FolderModel { Id = new ItemId("Some Folder") };
			var disc = new DiscModel { Id = new ItemId("Some Disc"), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc };

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(discFolder);

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
			var disc1 = new DiscModel { Id = new ItemId("Some Disc 1"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 1), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };
			var disc2 = new DiscModel { Id = new ItemId("Some Disc 2"), AdviseSetInfo = new AdviseSetInfo(adviseSet, 2), Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") } };

			var song1 = new SongModel { Id = new ItemId("1"), Disc = disc1 };
			var song2 = new SongModel { Id = new ItemId("2"), Disc = disc2 };

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(discFolder);

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

			var discFolder = new FolderModel
			{
				Id = new ItemId("Some Folder"),
			};

			var disc = new DiscModel
			{
				Id = new ItemId("Some Disc"),
				Folder = new ShallowFolderModel { Id = new ItemId("Some Folder") },
			};

			var mocker = new AutoMocker();
			mocker.GetMock<IFoldersService>()
				.Setup(x => x.GetFolder(new ItemId("Some Folder"), It.IsAny<CancellationToken>())).ReturnsAsync(discFolder);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(disc));

			// Assert

			var itemListViewModelMock = mocker.GetMock<ILibraryExplorerItemListViewModel>();

			itemListViewModelMock.Verify(x => x.LoadFolderItems(discFolder), Times.Once);
			itemListViewModelMock.Verify(x => x.SelectDisc(new ItemId("Some Disc")), Times.Once);
		}

		[TestMethod]
		public void SongChangedEventHandler_SongBelongsToSomeDisc_UpdatesSongInstance()
		{
			// Arrange

			var disc1 = new DiscModel
			{
				Id = new ItemId("Disc 1"),
				AllSongs = new[]
				{
					new SongModel
					{
						Id = new ItemId("Song 1"),
						Title = "Old Title 1",
					},
				},
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("Disc 2"),
				AllSongs = new[]
				{
					new SongModel
					{
						Id = new ItemId("Song 2"),
						Title = "Old Title 2",
					},
					new SongModel
					{
						Id = new ItemId("Song 3"),
						Title = "Old Title 3",
					},
				},
			};

			var changedSong = new SongModel
			{
				Id = new ItemId("Song 2"),
				Title = "New Title 2",
				Disc = new DiscModel { Id = new ItemId("Disc 2") },
			};

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.Discs).Returns(new[] { disc1, disc2 });

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new SongChangedEventArgs(changedSong, nameof(SongModel.Title)));

			// Assert

			var songs1 = disc1.AllSongs.ToList();
			songs1[0].Title.Should().Be("Old Title 1");

			var songs2 = disc2.AllSongs.ToList();
			songs2[0].Title.Should().Be("New Title 2");
			songs2[1].Title.Should().Be("Old Title 3");
		}

		[TestMethod]
		public void DiscChangedEventHandler_DiscPresentsInList_UpdatesDiscInstance()
		{
			// Arrange

			var disc1 = new DiscModel
			{
				Id = new ItemId("Disc 1"),
				Title = "Old Title 1",
			};

			var disc2 = new DiscModel
			{
				Id = new ItemId("Disc 2"),
				Title = "Old Title 2",
			};

			var disc3 = new DiscModel
			{
				Id = new ItemId("Disc 3"),
				Title = "Old Title 3",
			};

			var changedDisc = new DiscModel
			{
				Id = new ItemId("Disc 2"),
				Title = "New Title 2",
			};

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.Discs).Returns(new[] { disc1, disc2, disc3 });

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new DiscChangedEventArgs(changedDisc, nameof(DiscModel.Title)));

			// Assert

			disc1.Title.Should().Be("Old Title 1");
			disc2.Title.Should().Be("New Title 2");
			disc3.Title.Should().Be("Old Title 3");
		}

		[TestMethod]
		public void DiscImageChangedEventHandler_DiscPresentsInList_UpdatesDiscImage()
		{
			// Arrange

			var oldDiscImages1 = new[] { new DiscImageModel { Id = new ItemId("Old Image 1") } };
			var disc1 = new DiscModel
			{
				Id = new ItemId("Disc 1"),
				Images = oldDiscImages1,
			};

			var oldDiscImages2 = new[] { new DiscImageModel { Id = new ItemId("Old Image 2") } };
			var disc2 = new DiscModel
			{
				Id = new ItemId("Disc 2"),
				Images = oldDiscImages2,
			};

			var oldDiscImages3 = new[] { new DiscImageModel { Id = new ItemId("Old Image 3") } };
			var disc3 = new DiscModel
			{
				Id = new ItemId("Disc 3"),
				Images = oldDiscImages3,
			};

			var newDiscImages = new[] { new DiscImageModel { Id = new ItemId("New Image") } };
			var changedDisc = new DiscModel
			{
				Id = new ItemId("Disc 2"),
				Images = newDiscImages,
			};

			var mocker = new AutoMocker();
			mocker.GetMock<ILibraryExplorerItemListViewModel>()
				.Setup(x => x.Discs).Returns(new[] { disc1, disc2, disc3 });

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			Messenger.Default.Send(new DiscImageChangedEventArgs(changedDisc));

			// Assert

			disc1.Images.Should().BeEquivalentTo(oldDiscImages1, x => x.WithStrictOrdering());
			disc2.Images.Should().BeEquivalentTo(newDiscImages, x => x.WithStrictOrdering());
			disc3.Images.Should().BeEquivalentTo(oldDiscImages3, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfNoItemIsSelected_ReturnsEmptyCollection()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Some Advise Group" },
			};

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();
			target.SelectedFolder.Should().BeNull();
			target.SelectedDisc.Should().BeNull();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			menuItems.Should().BeEmpty();
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfFolderIsSelectedAndNoAdviseGroupsExist_ReturnsCorrectMenuItems()
		{
			// Arrange

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel());

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			menuItems.Should().HaveCount(1);
			menuItems.Single().Should().BeOfType(typeof(NewAdviseGroupMenuItem));
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfDiscIsSelectedAndNoAdviseGroupsExist_ReturnsCorrectMenuItems()
		{
			// Arrange

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedDisc).Returns(new DiscModel());

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			menuItems.Should().HaveCount(1);
			menuItems.Single().Should().BeOfType(typeof(NewAdviseGroupMenuItem));
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndFolderWithoutAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel());

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
				new SeparatorMenuItem(),
				new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[1], false, _ => Task.CompletedTask),
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndFolderWithAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var selectedFolder = new ShallowFolderModel
			{
				AdviseGroup = adviseGroups[1],
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(selectedFolder);

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
				new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroups[1], (_, _) => Task.CompletedTask),
				new SeparatorMenuItem(),
				new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[1], true, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[2], false, _ => Task.CompletedTask),
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfFolderIsSelected_CommandForSetAdviseGroupMenuItemReversesSelectedAdviseGroup()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel());

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperMock);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var menuItem = menuItems.OfType<SetAdviseGroupMenuItem>().ToList()[1];
			menuItem.Command.Execute(null);

			adviseGroupHelperMock.Verify(x => x.ReverseAdviseGroup(It.IsAny<FolderAdviseGroupHolder>(), adviseGroups[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndDiscWithoutAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedDisc).Returns(new DiscModel());

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
				new SeparatorMenuItem(),
				new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[1], false, _ => Task.CompletedTask),
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfAdviseGroupsExistAndDiscWithAdviseGroupIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var selectedDisc = new DiscModel
			{
				AdviseGroup = adviseGroups[1],
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedDisc).Returns(selectedDisc);

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new NewAdviseGroupMenuItem(_ => Task.CompletedTask),
				new ReverseFavoriteStatusForAdviseGroupMenuItem(adviseGroups[1], (_, _) => Task.CompletedTask),
				new SeparatorMenuItem(),
				new SetAdviseGroupMenuItem(adviseGroups[0], false, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[1], true, _ => Task.CompletedTask),
				new SetAdviseGroupMenuItem(adviseGroups[2], false, _ => Task.CompletedTask),
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfDiscIsSelected_CommandForSetAdviseGroupMenuItemReversesSelectedAdviseGroup()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
				new AdviseGroupModel { Id = new ItemId("3"), Name = "Advise Group 3" },
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedDisc).Returns(new DiscModel());

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperMock);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var menuItem = menuItems.OfType<SetAdviseGroupMenuItem>().ToList()[1];
			menuItem.Command.Execute(null);

			adviseGroupHelperMock.Verify(x => x.ReverseAdviseGroup(It.IsAny<DiscAdviseGroupHolder>(), adviseGroups[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfNewAdviseGroupMenuItemIsExecuted_ShowsShowCreateAdviseGroupViewCorrectly()
		{
			// Arrange

			var adviseGroups = new[]
			{
				new AdviseGroupModel { Id = new ItemId("1"), Name = "Advise Group 1" },
				new AdviseGroupModel { Id = new ItemId("2"), Name = "Advise Group 2" },
			};

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel { Name = "Folder Name" });

			var adviseGroupHelperStub = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperStub.Setup(x => x.AdviseGroups).Returns(adviseGroups);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
			menuItem.Command.Execute(null);

			var viewNavigatorMock = mocker.GetMock<IViewNavigator>();
			viewNavigatorMock.Verify(x => x.ShowCreateAdviseGroupView("Folder Name", new[] { "Advise Group 1", "Advise Group 2" }), Times.Once);
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfShowCreateAdviseGroupViewReturnsNull_DoesNotCreateAdviseGroup()
		{
			// Arrange

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel());

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var viewNavigatorStub = new Mock<IViewNavigator>();
			viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns<string>(null);

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperMock);
			mocker.Use(viewNavigatorStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
			menuItem.Command.Execute(null);

			adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(It.IsAny<BasicAdviseGroupHolder>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public void AdviseGroupMenuItems_IfShowCreateAdviseGroupViewReturnsNotNull_CreatesAdviseGroup()
		{
			// Arrange

			var itemListViewModelStub = new Mock<ILibraryExplorerItemListViewModel>();
			itemListViewModelStub.Setup(x => x.SelectedFolder).Returns(new ShallowFolderModel());

			var adviseGroupHelperMock = new Mock<IAdviseGroupHelper>();
			adviseGroupHelperMock.Setup(x => x.AdviseGroups).Returns(Array.Empty<AdviseGroupModel>());

			var viewNavigatorStub = new Mock<IViewNavigator>();
			viewNavigatorStub.Setup(x => x.ShowCreateAdviseGroupView(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("Some New Group");

			var mocker = new AutoMocker();
			mocker.Use(itemListViewModelStub);
			mocker.Use(adviseGroupHelperMock);
			mocker.Use(viewNavigatorStub);

			var target = mocker.CreateInstance<LibraryExplorerViewModel>();

			// Act

			var menuItems = target.AdviseGroupMenuItems;

			// Assert

			var menuItem = menuItems.OfType<NewAdviseGroupMenuItem>().Single();
			menuItem.Command.Execute(null);

			adviseGroupHelperMock.Verify(x => x.CreateAdviseGroup(It.IsAny<FolderAdviseGroupHolder>(), "Some New Group", It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
