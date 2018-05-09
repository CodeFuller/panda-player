using System;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels
{
	[TestFixture]
	public class EditDiscPropertiesViewModelTests
	{
		[Test]
		public void Constructor_WhenLibraryStructurerIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscPropertiesViewModel(null, Substitute.For<ILibraryContentUpdater>()));
		}

		[Test]
		public void Constructor_WhenLibraryContentUpdaterIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscPropertiesViewModel(Substitute.For<ILibraryStructurer>(), null));
		}

		[Test]
		public void Load_FillsInitialPropertiesCorrectly()
		{
			//	Arrange

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscFolderName(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative)).Returns("SomeDiscFolder");
			var target = new EditDiscPropertiesViewModel(libraryStructurerStub, Substitute.For<ILibraryContentUpdater>());

			//	Act

			target.Load(new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Title = "Some Disc",
				AlbumTitle = "Some Album",
			});

			//	Assert

			Assert.AreEqual("SomeDiscFolder", target.FolderName);
			Assert.AreEqual("Some Disc", target.DiscTitle);
			Assert.AreEqual("Some Album", target.AlbumTitle);
		}

		[Test]
		public void Save_IfFolderNameWasChanged_ChangesDiscUri()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Title = "Some Disc",
				AlbumTitle = "Some Album",
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscFolderName(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative)).Returns("SomeDiscFolder");
			libraryStructurerStub.ReplaceDiscPartInUri(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative), "SomeNewDiscFolder")
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative));

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			var target = new EditDiscPropertiesViewModel(libraryStructurerStub, libraryContentUpdaterMock);

			target.Load(disc);

			//	Act

			target.FolderName = "SomeNewDiscFolder";
			target.Save().Wait();

			//	Assert

			libraryContentUpdaterMock.Received(1).ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative));
		}

		[Test]
		public void Save_IfFolderNameWasNotChanged_DoesNotChangeDiscUri()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Title = "Some Disc",
				AlbumTitle = "Some Album",
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscFolderName(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative)).Returns("SomeDiscFolder");

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			var target = new EditDiscPropertiesViewModel(libraryStructurerStub, libraryContentUpdaterMock);

			target.Load(disc);

			//	Act

			target.Save().Wait();

			//	Assert

			libraryContentUpdaterMock.DidNotReceive().ChangeDiscUri(Arg.Any<Disc>(), Arg.Any<Uri>());
		}

		[Test]
		public void Save_WhenAlbumTitleWasChanged_UpdatesDiscCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Title = "Some Disc",
				AlbumTitle = "Some Album",
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscFolderName(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative)).Returns("SomeDiscFolder");

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			var target = new EditDiscPropertiesViewModel(libraryStructurerStub, libraryContentUpdaterMock);

			target.Load(disc);

			//	Act

			target.AlbumTitle = "Some New Album";
			target.Save().Wait();

			//	Assert

			libraryContentUpdaterMock.Received(1).UpdateDisc(disc, UpdatedSongProperties.Album);
		}

		[Test]
		public void Save_WhenAlbumTitleWasNotChanged_UpdatesDiscCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Title = "Some Disc",
				AlbumTitle = "Some Album",
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscFolderName(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative)).Returns("SomeDiscFolder");

			ILibraryContentUpdater libraryContentUpdaterMock = Substitute.For<ILibraryContentUpdater>();
			var target = new EditDiscPropertiesViewModel(libraryStructurerStub, libraryContentUpdaterMock);

			target.Load(disc);

			//	Act

			target.Save().Wait();

			//	Assert

			libraryContentUpdaterMock.Received(1).UpdateDisc(disc, UpdatedSongProperties.None);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void FolderNameSetter_WhenFolderNameIsEmpty_ThrowsInvalidOperationException(string newFolderName)
		{
			//	Arrange

			var target = new EditDiscPropertiesViewModel(Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.FolderName = newFolderName);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void DiscTitleSetter_WhenDiscTitleIsEmpty_ThrowsInvalidOperationException(string newDiscTitle)
		{
			//	Arrange

			var target = new EditDiscPropertiesViewModel(Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.DiscTitle = newDiscTitle);
		}

		[Test]
		public void AlbumTitleSetter_WhenAlbumTitleIsEmpty_SetsTitleToNull()
		{
			//	Arrange

			var target = new EditDiscPropertiesViewModel(Substitute.For<ILibraryStructurer>(), Substitute.For<ILibraryContentUpdater>());

			//	Act

			target.AlbumTitle = "";

			//	Assert

			Assert.IsNull(target.AlbumTitle);
		}
	}
}
