using System;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
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
			Assert.Throws<ArgumentNullException>(() => new DiscArtViewModel(null));
		}

		[Test]
		public void CurrImageFileName_IfNoActiveDiscSet_ReturnsNull()
		{
			//	Arrange

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(Arg.Any<Disc>()).Returns("SomeCover.jpg");
			var target = new DiscArtViewModel(musicLibraryStub);

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
			var target = new DiscArtViewModel(musicLibraryStub);

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
			var target = new DiscArtViewModel(musicLibraryStub);

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
			var target = new DiscArtViewModel(musicLibraryStub);

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
			var target = new DiscArtViewModel(musicLibraryStub);

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
			var target = new DiscArtViewModel(musicLibraryStub);

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(disc));
			var currImageFileName = target.CurrImageFileName;

			//	Assert

			Assert.IsNull(currImageFileName);
		}
	}
}
