using System;
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
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class LibraryExplorerViewModelTests
	{
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

			bool receivedEvent = false;
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => receivedEvent = (e.Disc == null));

			ILibraryContentUpdater libraryContentUpdater = Substitute.For<ILibraryContentUpdater>();
			libraryContentUpdater.When(x => x.DeleteDisc(Arg.Any<Disc>())).Do(x =>
			{
				if (!receivedEvent)
				{
					throw new InvalidOperationException(Current($"{nameof(LibraryExplorerDiscChangedEventArgs)} should be sent before disc deletion"));
				}
			});

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<IExplorerSongListViewModel>(),
				libraryContentUpdater, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void DeleteDisc_IfDiscIsDeleted_RemovesDiscItemFromLibraryBrowser()
		{
			//	Arrange

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			});

			var folderItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			ILibraryBrowser libraryBrowserMock = Substitute.For<ILibraryBrowser>();
			libraryBrowserMock.GetChildFolderItems(folderItem).Returns(new[] { discItem });

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserMock, Substitute.For<IExplorerSongListViewModel>(),
				Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolder();
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDisc().Wait();

			//	Assert

			libraryBrowserMock.Received(1).RemoveDiscItem(discItem);
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

			bool discDeleted = false;
			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem1).Returns(x => !discDeleted ? new[] { folderItem21, folderItem22 } : new[] { folderItem22 });
			libraryBrowserStub.GetChildFolderItems(folderItem21).Returns(x => !discDeleted ? new[] { discItem } : new DiscExplorerItem[] { });
			libraryBrowserStub.GetParentFolder(discItem).Returns(folderItem21);
			libraryBrowserStub.GetParentFolder(folderItem21).Returns(folderItem1);
			libraryBrowserStub.RemoveDiscItem(Arg.Do<DiscExplorerItem>(arg => discDeleted = true));

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

			bool discDeleted = false;
			ILibraryBrowser libraryBrowserStub = Substitute.For<ILibraryBrowser>();
			libraryBrowserStub.GetChildFolderItems(folderItem2).Returns(x => !discDeleted ? new[] { discItem1, discItem2 } : new DiscExplorerItem[] { discItem2 });
			libraryBrowserStub.GetParentFolder(discItem1).Returns(folderItem2);
			libraryBrowserStub.GetParentFolder(folderItem2).Returns(folderItem1);
			libraryBrowserStub.RemoveDiscItem(Arg.Do<DiscExplorerItem>(arg => discDeleted = true));

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
