using System;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class LibraryExplorerViewModelTests
	{
		[Test]
		public void DeleteDiscCommand_IfSelectedItemIsNotADisc_ReturnsWithNoAction()
		{
			//	Arrange

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = new FolderExplorerItem(new Uri("/SomeFolder", UriKind.Relative));

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			libraryContentUpdaterMock.DidNotReceive().DeleteDisc(Arg.Any<Disc>());
		}

		[Test]
		public void DeleteDiscCommand_IfUserCancelsDeletion_ReturnsWithNoAction()
		{
			//	Arrange

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.No);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			} );

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			libraryContentUpdaterMock.DidNotReceive().DeleteDisc(Arg.Any<Disc>());
		}

		[Test]
		public void DeleteDiscCommand_IfUserConfirmsDeletion_DeletesDiscCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			};

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(Substitute.For<ILibraryBrowser>(), libraryContentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = new DiscExplorerItem(disc);

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			libraryContentUpdaterMock.Received(1).DeleteDisc(disc);
		}

		[Test]
		public void DeleteDiscCommand_IfDiscIsDeleted_RemovesDiscItemFromLibraryBrowser()
		{
			//	Arrange

			var discItem = new DiscExplorerItem(new Disc
			{
				Title = "Some title",
				Uri = new Uri("/SomeFolder", UriKind.Relative),
			});

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			ILibraryBrowser libraryBrowserMock = Substitute.For<ILibraryBrowser>();
			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserMock, Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			libraryBrowserMock.Received(1).RemoveDiscItem(discItem);
		}

		[Test]
		public void DeleteDiscCommand_IfDiscIsDeleted_RemovesDiscFromItemsList()
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

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem;
			target.ChangeFolderCommand.Execute(null);
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			Assert.AreEqual(0, target.Items.Count);
		}

		[Test]
		public void DeleteDiscCommand_IfCurrentFolderContainsOtherDiscs_DoesNotChangeCurrentFolder()
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

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem21;
			target.ChangeFolderCommand.Execute(null);
			target.SelectedItem = discItem;

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			Assert.AreSame(folderItem1, target.CurrentFolder);
		}

		[Test]
		public void DeleteDiscCommand_IfCurrentFolderBecomesEmpty_SwitchesToParentFolder()
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

			LibraryExplorerViewModel target = new LibraryExplorerViewModel(libraryBrowserStub, Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SelectedItem = folderItem2;
			target.ChangeFolderCommand.Execute(null);
			target.SelectedItem = discItem1;

			//	Act

			target.DeleteDiscCommand.Execute(null);

			//	Assert

			Assert.AreSame(folderItem2, target.CurrentFolder);
		}
	}
}
