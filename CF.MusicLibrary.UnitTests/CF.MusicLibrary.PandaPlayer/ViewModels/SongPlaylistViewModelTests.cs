using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class SongPlaylistViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void DisplayTrackNumbersGetter_ReturnsFalse()
		{
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			Assert.IsFalse(target.DisplayTrackNumbers);
		}

		[Test]
		public void PlayedDiscGetter_IfAllSongsBelongToOneDisc_ReturnsThisDisc()
		{
			//	Arrange

			var disc = new Disc();
			var songs = new List<Song>
			{
				new Song { Disc = disc },
				new Song { Disc = disc }
			};

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(songs);

			//	Act

			var playedDisc = target.PlayedDisc;

			//	Assert

			Assert.AreSame(disc, playedDisc);
		}

		[Test]
		public void PlayedDiscGetter_IfSongsBelongToDifferentDiscs_ReturnsNull()
		{
			//	Arrange

			var songs = new List<Song>
			{
				new Song { Disc = new Disc() },
				new Song { Disc = new Disc() },
			};

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(songs);

			//	Act

			var playedDisc = target.PlayedDisc;

			//	Assert

			Assert.IsNull(playedDisc);
		}

		[Test]
		public void SetSongs_WhenSongListIsChanged_SendsPlaylistChangedEventWithUpdatedSongs()
		{
			//	Arrange

			var songs = new List<Song> { new Song() };
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			List<Song> registeredSongs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredSongs = e.Playlist.Songs.ToList());

			//	Act

			target.SetSongs(songs);

			//	Assert

			Assert.IsNotNull(registeredSongs);
			CollectionAssert.AreEqual(songs, registeredSongs);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsNotSet_SetsFirstSongAsCurrent()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.AreSame(song1, target.CurrentSong);
			Assert.AreEqual(0, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsSet_SetsNextSongAsCurrent()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.AreSame(song2, target.CurrentSong);
			Assert.AreEqual(1, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingToNextSong_SetsItsIsCurrentlyPlayedPropertyToTrue()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.IsTrue(target.SongItems[0].IsCurrentlyPlayed);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingFromCurrentSong_SetsItsIsCurrentlyPlayedPropertyToFalse()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.IsFalse(target.SongItems[0].IsCurrentlyPlayed);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingToNextSong_SendsPlaylistChangedEventWithNewCurrentSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			Song registeredCurrentSong = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredCurrentSong = e.Playlist.CurrentSong);

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.IsNotNull(registeredCurrentSong);
			Assert.AreSame(song2, registeredCurrentSong);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsTheLast_SetsCurrentSongToNull()
		{
			//	Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { new Song() });
			target.SwitchToNextSong();

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.IsNull(target.CurrentSong);
			Assert.IsNull(target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToSong_WhenSongPresetnInPlaylist_SwitchesToSpecifiedSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });

			//	Act

			target.SwitchToSong(song2);

			//	Assert

			Assert.AreSame(song2, target.CurrentSong);
			Assert.AreEqual(1, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToSong_WhenSwitchingToSpecifiedSong_SendsPlaylistChangedEventWithNewCurrentSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });

			Song registeredCurrentSong = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredCurrentSong = e.Playlist.CurrentSong);

			//	Act

			target.SwitchToSong(song2);

			//	Assert

			Assert.IsNotNull(registeredCurrentSong);
			Assert.AreSame(song2, registeredCurrentSong);
		}

		[Test]
		public void SwitchToSong_WhenMultipleSongsMatchInPlaylist_ThrowsInvalidOperatioException()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2, song2 });

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.SwitchToSong(song2));
		}

		[Test]
		public void SwitchToSong_WhenNoSongsMatchInPlaylist_ThrowsInvalidOperatioException()
		{
			//	Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new List<Song> { new Song() });

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.SwitchToSong(new Song()));
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsNotEmptyAndCurrentSongIsSet_AddsNewSongsAfterCurrentSong()
		{
			//	Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { oldSong1, oldSong2 });
			target.SwitchToNextSong();

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			//	Assert

			CollectionAssert.AreEqual(target.Songs, new[] { oldSong1, newSong1, newSong2, oldSong2 });
			Assert.AreSame(target.CurrentSong, oldSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsNotEmptyAndCurrentSongIsNotSet_AddsNewSongsAtListTop()
		{
			//	Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { oldSong1, oldSong2 });

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			//	Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2, oldSong1, oldSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsEmpty_FillsListWithNewSongs()
		{
			//	Arrange

			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			//	Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_AfterUpdatingPlaylist_SendsPlaylistChangedEvent()
		{
			//	Arrange

			var newSongs = new[] { new Song() };

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			List<Song> updatedSongList = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => updatedSongList = e.Playlist.Songs.ToList());

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(newSongs));

			//	Assert

			Assert.IsNotNull(updatedSongList);
			CollectionAssert.AreEqual(newSongs, updatedSongList);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_WhenSongListIsNotEmpty_AddsNewSongsAtListEnd()
		{
			//	Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { oldSong1, oldSong2 });
			target.SwitchToNextSong();

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(new[] { newSong1, newSong2 }));

			//	Assert

			CollectionAssert.AreEqual(target.Songs, new[] { oldSong1, oldSong2, newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, oldSong1);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_WhenSongListIsEmpty_FillsListWithNewSongs()
		{
			//	Arrange

			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(new[] { newSong1, newSong2 }));

			//	Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_AfterUpdatingPlaylist_SendsPlaylistChangedEvent()
		{
			//	Arrange

			var newSongs = new[] {new Song()};

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			List<Song> updatedSongList = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => updatedSongList = e.Playlist.Songs.ToList());

			//	Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(newSongs));

			//	Assert

			Assert.IsNotNull(updatedSongList);
			CollectionAssert.AreEqual(newSongs, updatedSongList);
		}

		[Test]
		public void PlayFromSongCommand_IfSomeSongIsSelected_SendsPlayPlaylistStartingFromSongEventForSelectedSong()
		{
			//	Arrange

			var song = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SelectedSongItem = new SongListItem(song);

			PlayPlaylistStartingFromSongEventArgs receivedEvent = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlayFromSong();

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(song, receivedEvent.Song);
		}

		[Test]
		public void PlayFromSongCommand_IfNoSongIsSelected_DoesNotSendPlayPlaylistStartingFromSongEvent()
		{
			//	Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			PlayPlaylistStartingFromSongEventArgs receivedEvent = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlayFromSong();

			//	Assert

			Assert.IsNull(receivedEvent);
			//	Avoiding uncovered lambda code (e => receivedEvent = e)
			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs(null));
		}
	}
}
