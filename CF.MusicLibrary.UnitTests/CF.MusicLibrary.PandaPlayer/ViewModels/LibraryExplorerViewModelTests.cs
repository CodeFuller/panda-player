using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class LibraryExplorerViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void ItemsGetter_ReturnsItemsSortedByName()
		{
			//	Arrange

			var item1 = new FolderExplorerItem(new Uri("/SomeFolder/Item 1", UriKind.Relative));
			var item2 = new FolderExplorerItem(new Uri("/SomeFolder/Item 2", UriKind.Relative));
			var item3 = new FolderExplorerItem(new Uri("/SomeFolder/Item 3", UriKind.Relative));
			//	Sanity check
			Assert.AreEqual("Item 1", item1.Name);

			var unsortedItems = new[] { item2, item1, item3 };
			var sortedItems = new[] { item1, item2, item3 };

			var parentItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(parentItem).Returns(unsortedItems);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SelectedItem = parentItem;
			target.ChangeFolder();

			//	Act & Assert

			CollectionAssert.AreEqual(sortedItems, target.Items);
		}

		[Test]
		public void SelectedItemSetter_WhenNewItemIsDisc_LoadsSongListWithSongsFromNewDisc()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative),
				SongsUnordered = new[] { new Song(), new Song() },
			};

			List<Song> newSongs = null;
			IExplorerSongListViewModel explorerSongListMock = Substitute.For<IExplorerSongListViewModel>();
			explorerSongListMock.SetSongs(Arg.Do<IEnumerable<Song>>(arg => newSongs = arg.ToList()));

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), explorerSongListMock,
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			//	Act

			target.SelectedItem = new DiscExplorerItem(disc);

			//	Assert

			Assert.IsNotNull(newSongs);
			CollectionAssert.AreEqual(disc.Songs, newSongs);
		}

		[Test]
		public void SelectedItemSetter_WhenNewItemIsDisc_SendsLibraryExplorerDiscChangedEventForNewDisc()
		{
			//	Arrange

			var disc = new Disc { Uri = new Uri("/SomeDisc", UriKind.Relative) };

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			LibraryExplorerDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.SelectedItem = new DiscExplorerItem(disc);

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(disc, receivedEvent.Disc);
		}

		[Test]
		public void SelectedItemSetter_WhenNewItemIsNotDisc_ClearsSongsList()
		{
			//	Arrange

			List<Song> newSongs = null;
			IExplorerSongListViewModel explorerSongListMock = Substitute.For<IExplorerSongListViewModel>();
			explorerSongListMock.SetSongs(Arg.Do<IEnumerable<Song>>(arg => newSongs = arg.ToList()));

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), explorerSongListMock,
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			//	Act

			target.SelectedItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			//	Assert

			Assert.IsNotNull(newSongs);
			CollectionAssert.IsEmpty(newSongs);
		}

		[Test]
		public void SelectedItemSetter_WhenNewItemIsNotDisc_SendsLibraryExplorerDiscChangedEventWithNullDisc()
		{
			//	Arrange

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			LibraryExplorerDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.SelectedItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.IsNull(receivedEvent.Disc);
		}

		[Test]
		public void DeleteDisc_IfSelectedItemIsNotADisc_ReturnsWithNoAction()
		{
			//	Arrange

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), Substitute.For<IExplorerSongListViewModel>(),
				libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			libraryContentUpdaterMock.DidNotReceive().DeleteDisc(Arg.Any<Disc>());
		}

		[Test]
		public void DeleteDisc_IfUserCancelsDeletion_ReturnsWithNoAction()
		{
			//	Arrange

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.No);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), Substitute.For<IExplorerSongListViewModel>(),
				libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			} );

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			libraryContentUpdaterMock.DidNotReceive().DeleteDisc(Arg.Any<Disc>());
		}

		[Test]
		public void DeleteDisc_IfUserConfirmsDeletion_DeletesDiscCorrectly()
		{
			//	Arrange

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			});

			var folderItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem).Returns(new[] { discItem });

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			libraryContentUpdaterMock.Received(1).DeleteDisc(discItem.Disc);
		}

		[Test]
		public void DeleteDisc_IfDiscIsDeleted_SendsLibraryExplorerDiscChangedEventBeforeDiscDeletion()
		{
			//	Arrange

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			});

			var folderItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem).Returns(new[] { discItem });

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			int currCallOrder = 1;
			int? receivedEventOrder = null;
			int? deleteDiscOrder = null;
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => receivedEventOrder = currCallOrder++);

			ILibraryContentUpdater libraryContentUpdater = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdater.When(x => x.DeleteDisc(Arg.Any<Disc>())).Do(x => deleteDiscOrder = currCallOrder++);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				libraryContentUpdater, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			currCallOrder = 1;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			Assert.AreEqual(1, receivedEventOrder);
			Assert.AreEqual(2, deleteDiscOrder);
		}

		[Test]
		public void DeleteDisc_IfDiscIsDeleted_RemovesDiscFromItemsList()
		{
			//	Arrange

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder/SomeDisc", UriKind.Relative),
			});

			var folderItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem).Returns(new[] { discItem });

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			Assert.AreEqual(0, target.Items.Count);
		}

		[Test]
		public void DeleteDisc_IfCurrentFolderContainsOtherDiscs_DoesNotChangeCurrentFolder()
		{
			//	Arrange

			var folderItem1 = new FolderExplorerItem(new Uri("/SomeFolder1", UriKind.Relative));
			var folderItem21 = new FolderExplorerItem(new Uri("/SomeFolder1/SomeFolder21", UriKind.Relative));
			var folderItem22 = new FolderExplorerItem(new Uri("/SomeFolder1/SomeFolder22", UriKind.Relative));

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder1/SomeFolder12/SomeDisc", UriKind.Relative),
			});

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem1).Returns(new[] { folderItem21, folderItem22 }.AsEnumerable(), new[] { folderItem22 }.AsEnumerable());
			libraryBrowserStub.GetChildFolderItems(folderItem21).Returns(new[] { discItem }.AsEnumerable(), Enumerable.Empty<DiscExplorerItem>());
			libraryBrowserStub.GetParentFolder(discItem).Returns(folderItem21);
			libraryBrowserStub.GetParentFolder(folderItem21).Returns(folderItem1);

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem21;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			Assert.AreSame(folderItem1, target.CurrentFolder);
		}

		[Test]
		public void DeleteDisc_IfCurrentFolderBecomesEmpty_SwitchesToParentFolder()
		{
			//	Arrange

			var folderItem1 = new FolderExplorerItem(new Uri("/SomeFolder1", UriKind.Relative));
			var folderItem2 = new FolderExplorerItem(new Uri("/SomeFolder1/SomeFolder21", UriKind.Relative));

			var discItem1 = new DiscExplorerItem(new Disc
			{
				Title = "Disc 1",
				Uri = new Uri("/SomeFolder1/SomeFolder12/SomeDisc1", UriKind.Relative),
			});

			var discItem2 = new DiscExplorerItem(new Disc
			{
				Title = "Disc 2",
				Uri = new Uri("/SomeFolder1/SomeFolder12/SomeDisc2", UriKind.Relative),
			});

			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem2).Returns(new[] { discItem1, discItem2 }.AsEnumerable(), new[] { discItem2 }.AsEnumerable());
			libraryBrowserStub.GetParentFolder(discItem1).Returns(folderItem2);
			libraryBrowserStub.GetParentFolder(folderItem2).Returns(folderItem1);

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem2;
			target.ChangeFolder();
			target.SelectedItem = discItem1;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			Assert.AreSame(folderItem2, target.CurrentFolder);
		}
	}
}
