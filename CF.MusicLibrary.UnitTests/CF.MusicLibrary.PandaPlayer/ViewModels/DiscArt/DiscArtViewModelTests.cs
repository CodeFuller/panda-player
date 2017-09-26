using System;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	[TestFixture]
	public class DiscArtViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtViewModel(null, Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfViewNavigatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtViewModel(Substitute.For<IMusicLibrary>(), null));
		}

		[Test]
		public void CurrImageFileName_IfNoActiveDiscSet_ReturnsNull()
		{
			//	Arrange

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(Arg.Any<Disc>()).Returns("SomeCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.IsNull(currImageFileName);
		}

		[Test]
		public void CurrImageFileName_AfterCurrExplorerItemSetToDiscWhenNoSongIsPlayed_ReturnsCoverImageOfExplorerDisc()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act
			
			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.AreEqual("SomeCover.jpg", currImageFileName);
		}

		[Test]
		public void CurrImageFileName_AfterCurrExplorerItemSetToFolderWhenNoSongIsPlayed_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			Messenger.Default.Send(new LibraryExplorerFolderChangedEventArgs(new Uri("/SomeUri", UriKind.Relative)));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.IsNull(currImageFileName);
		}

		[Test]
		public void CurrImageFileName_AfterPlayedSongChanges_ReturnsCoverImageOfSongDisc()
		{
			//	Arrange

			var disc = new Disc();
			var songDisc = new Disc();
			var song = new Song { Disc = songDisc };

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.CurrentSong.Returns(song);

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeDiscCover.jpg");
			musicLibraryStub.GetDiscCoverImage(songDisc).Returns("SomeSongDiscCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistStub));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.AreEqual("SomeSongDiscCover.jpg", currImageFileName);
		}

		[Test]
		public void CurrImageFileName_AfterPlaylistFinishes_ReturnsCoverImageOfExplorerDisc()
		{
			//	Arrange

			var disc = new Disc();
			var songDisc = new Disc();
			var song = new Song { Disc = songDisc };

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.CurrentSong.Returns(song);

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns("SomeDiscCover.jpg");
			musicLibraryStub.GetDiscCoverImage(songDisc).Returns("SomeSongDiscCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistStub));
			Messenger.Default.Send(new PlaylistFinishedEventArgs(songPlaylistStub));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.AreEqual("SomeDiscCover.jpg", currImageFileName);
		}

		[Test]
		public void CurrImageFileName_IfActiveDiscDoesNotHaveCoverImage_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns((string)null);
			var target = new DiscArtViewModel(musicLibraryStub, Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.IsNull(currImageFileName);
		}

		[Test]
		public void EditDiscArt_IfNoActiveDiscSet_ReturnsWithNoAction()
		{
			//	Arrange

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			//	Act

			target.EditDiscArt().Wait();

			//	Assert

			viewNavigatorMock.DidNotReceive().ShowEditDiscArtView(Arg.Any<Disc>());
		}

		[Test]
		public void EditDiscArt_IfActiveDiscIsCurrExplorerItem_ShowEditDiscArtViewForExplorerDisc()
		{
			//	Arrange

			var disc = new Disc();

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));

			//	Act

			target.EditDiscArt().Wait();

			//	Assert

			viewNavigatorMock.Received(1).ShowEditDiscArtView(disc);
		}

		[Test]
		public void EditDiscArt_IfActiveDiscIsCurrSongDisc_ShowEditDiscArtViewForCurrSongDisc()
		{
			//	Arrange

			var disc = new Disc();
			var songDisc = new Disc();
			var song = new Song { Disc = songDisc };

			ISongPlaylistViewModel songPlaylistStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistStub.CurrentSong.Returns(song);

			IViewNavigator viewNavigatorMock = Substitute.For<IViewNavigator>();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), viewNavigatorMock);

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistStub));

			//	Act

			target.EditDiscArt().Wait();

			//	Assert

			viewNavigatorMock.Received(1).ShowEditDiscArtView(songDisc);
		}

		[Test]
		public void DiscArtChangedEventHandler_IfChangedDiscIsActiveDisc_RaisesPropertyChangedEventForCurrImageFileName()
		{
			//	Arrange

			var disc = new Disc();
			var target = new DiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IViewNavigator>());
			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));

			bool raisedPropertyChangedEvent = false;
			target.PropertyChanged += (sender, e) => raisedPropertyChangedEvent = (e.PropertyName == nameof(DiscArtViewModel.CurrImageFileName) || raisedPropertyChangedEvent);

			//	Act

			Messenger.Default.Send(new DiscArtChangedEventArgs(disc));

			//	Assert

			Assert.IsTrue(raisedPropertyChangedEvent);
		}
	}
}
