using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests.ViewModels
{
	[TestFixture]
	public class SongPlaylistViewModelTests
	{
		private class SongPlaylistViewModelInheritor : SongPlaylistViewModel
		{
			public int OnPlaylistChangedCallsNumber { get; set; }

			private List<Song> songsWhenOnPlaylistChangedCalled;

			public IReadOnlyCollection<Song> SongsWhenOnPlaylistChangedCalled => songsWhenOnPlaylistChangedCalled;

			public Song CurrentSongWhenOnPlaylistChangedCalled { get; set; }

			public SongPlaylistViewModelInheritor(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
				: base(libraryContentUpdater, viewNavigator, windowService)
			{
			}

			protected override void OnPlaylistChanged()
			{
				songsWhenOnPlaylistChangedCalled = Songs.ToList();
				CurrentSongWhenOnPlaylistChangedCalled = CurrentSong;
				++OnPlaylistChangedCallsNumber;
			}

			public void InvokeOnPlaylistChanged()
			{
				base.OnPlaylistChanged();
			}
		}

		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void DisplayTrackNumbersGetter_ReturnsFalse()
		{
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			Assert.IsFalse(target.DisplayTrackNumbers);
		}

		[Test]
		public void PlayedDiscGetter_IfAllSongsBelongToOneDisc_ReturnsThisDisc()
		{
			// Arrange

			var disc = new Disc();
			var songs = new List<Song>
			{
				new Song { Disc = disc },
				new Song { Disc = disc },
			};

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(songs);

			// Act

			var playedDisc = target.PlayedDisc;

			// Assert

			Assert.AreSame(disc, playedDisc);
		}

		[Test]
		public void PlayedDiscGetter_IfSongsBelongToDifferentDiscs_ReturnsNull()
		{
			// Arrange

			var songs = new List<Song>
			{
				new Song { Disc = new Disc() },
				new Song { Disc = new Disc() },
			};

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(songs);

			// Act

			var playedDisc = target.PlayedDisc;

			// Assert

			Assert.IsNull(playedDisc);
		}

		[Test]
		public void SetSongs_IfCurrentSongIndexIsGreaterThanSizeOfNewSongList_UpdatesSongListCorrectly()
		{
			// Arrange

			var currSong = new Song();
			var songs1 = new List<Song> { new Song(), currSong };
			var songs2 = new List<Song> { new Song() };

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(songs1);
			target.SwitchToSong(currSong);

			// Act

			target.SetSongs(songs2);

			// Assert

			CollectionAssert.AreEqual(songs2, target.Songs);
		}

		[Test]
		public void SetSongs_AfterSongListIsChanged_CallsOnPlaylistChanged()
		{
			// Arrange

			var songs = new List<Song> { new Song() };
			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			target.SetSongs(songs);

			// Assert

			CollectionAssert.AreEqual(songs, target.SongsWhenOnPlaylistChangedCalled);
		}

		[Test]
		public void SetSongs_WhenSongListIsChanged_CallsOnPlaylistChangedOnlyOnce()
		{
			// Arrange

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song() });
			target.SwitchToNextSong();
			target.OnPlaylistChangedCallsNumber = 0;

			// Act

			target.SetSongs(new[] { new Song(), new Song() });

			// Assert

			Assert.AreEqual(1, target.OnPlaylistChangedCallsNumber);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsNotSet_SetsFirstSongAsCurrent()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.AreSame(song1, target.CurrentSong);
			Assert.AreEqual(0, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsSet_SetsNextSongAsCurrent()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.AreSame(song2, target.CurrentSong);
			Assert.AreEqual(1, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingToNextSong_SetsItsIsCurrentlyPlayedPropertyToTrue()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.IsTrue(target.SongItems[0].IsCurrentlyPlayed);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingFromCurrentSong_SetsItsIsCurrentlyPlayedPropertyToFalse()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.IsFalse(target.SongItems[0].IsCurrentlyPlayed);
		}

		[Test]
		public void SwitchToNextSong_WhenSwitchingToNextSong_CallsOnPlaylistChangedCorrectly()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();
			target.CurrentSongWhenOnPlaylistChangedCalled = null;

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.AreSame(song2, target.CurrentSongWhenOnPlaylistChangedCalled);
		}

		[Test]
		public void SwitchToNextSong_IfCurrentSongIsTheLast_SetsCurrentSongToNull()
		{
			// Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { new Song() });
			target.SwitchToNextSong();

			// Act

			target.SwitchToNextSong();

			// Assert

			Assert.IsNull(target.CurrentSong);
			Assert.IsNull(target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToSong_WhenSongPresetnInPlaylist_SwitchesToSpecifiedSong()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });

			// Act

			target.SwitchToSong(song2);

			// Assert

			Assert.AreSame(song2, target.CurrentSong);
			Assert.AreEqual(1, target.CurrentSongIndex);
		}

		[Test]
		public void SwitchToSong_WhenSwitchingToSpecifiedSong_CallsOnPlaylistChangedCorrectly()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.CurrentSongWhenOnPlaylistChangedCalled = null;

			// Act

			target.SwitchToSong(song2);

			// Assert

			Assert.AreSame(song2, target.CurrentSongWhenOnPlaylistChangedCalled);
		}

		[Test]
		public void SwitchToSong_WhenMultipleSongsMatchInPlaylist_ThrowsInvalidOperatioException()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			target.SetSongs(new List<Song> { song1, song2, song2 });

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.SwitchToSong(song2));
		}

		[Test]
		public void SwitchToSong_WhenNoSongsMatchInPlaylist_ThrowsInvalidOperatioException()
		{
			// Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new List<Song> { new Song() });

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.SwitchToSong(new Song()));
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsNotEmptyAndCurrentSongIsSet_AddsNewSongsAfterCurrentSong()
		{
			// Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { oldSong1, oldSong2 });
			target.SwitchToNextSong();

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			// Assert

			CollectionAssert.AreEqual(target.Songs, new[] { oldSong1, newSong1, newSong2, oldSong2 });
			Assert.AreSame(target.CurrentSong, oldSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsNotEmptyAndCurrentSongIsNotSet_AddsNewSongsAtListTop()
		{
			// Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { oldSong1, oldSong2 });

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			// Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2, oldSong1, oldSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_WhenSongListIsEmpty_FillsListWithNewSongs()
		{
			// Arrange

			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new[] { newSong1, newSong2 }));

			// Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistNextEventHandler_AfterUpdatingPlaylist_CallsOnPlaylistChangedCorrectly()
		{
			// Arrange

			var newSongs = new[] { new Song() };

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(newSongs));

			// Assert

			CollectionAssert.AreEqual(newSongs, target.SongsWhenOnPlaylistChangedCalled);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_WhenSongListIsNotEmpty_AddsNewSongsAtListEnd()
		{
			// Arrange

			Song oldSong1 = new Song();
			Song oldSong2 = new Song();
			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { oldSong1, oldSong2 });
			target.SwitchToNextSong();

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(new[] { newSong1, newSong2 }));

			// Assert

			CollectionAssert.AreEqual(target.Songs, new[] { oldSong1, oldSong2, newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, oldSong1);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_WhenSongListIsEmpty_FillsListWithNewSongs()
		{
			// Arrange

			Song newSong1 = new Song();
			Song newSong2 = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(new[] { newSong1, newSong2 }));

			// Assert

			CollectionAssert.AreEqual(target.Songs, new[] { newSong1, newSong2 });
			Assert.AreSame(target.CurrentSong, newSong1);
		}

		[Test]
		public void AddingSongsToPlaylistLastEventHandler_AfterUpdatingPlaylist_CallsOnPlaylistChangedCorrectly()
		{
			// Arrange

			var newSongs = new[] { new Song() };

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(newSongs));

			// Assert

			CollectionAssert.AreEqual(newSongs, target.SongsWhenOnPlaylistChangedCalled);
		}

		[Test]
		public void PlayFromSongCommand_IfSomeSongIsSelected_SetsCurrentSongToSelectedOne()
		{
			// Arrange

			var song = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { song });
			target.SelectedSongItem = target.SongItems[0];

			// Sanity check
			Assert.IsNull(target.CurrentSong);

			// Act

			target.PlayFromSong();

			// Assert

			Assert.AreSame(song, target.CurrentSong);
		}

		[Test]
		public void PlayFromSongCommand_IfSongIsAddedToPlaylistMultipleTimes_SetsCurrentSongToSelectedOne()
		{
			// Arrange

			var song = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { song, song });
			target.SelectedSongItem = target.SongItems[1];

			// Act

			target.PlayFromSong();

			// Assert

			Assert.AreEqual(1, target.CurrentSongIndex);
		}

		[Test]
		public void PlayFromSongCommand_IfSomeSongIsSelected_SendsPlayPlaylistStartingFromSongEventForSelectedSong()
		{
			// Arrange

			var song = new Song();

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { song });
			target.SelectedSongItem = target.SongItems[0];

			PlayPlaylistStartingFromSongEventArgs receivedEvent = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => receivedEvent = e);

			// Act

			target.PlayFromSong();

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(song, receivedEvent.Song);
		}

		[Test]
		public void PlayFromSongCommand_IfNoSongIsSelected_DoesNotSendPlayPlaylistStartingFromSongEvent()
		{
			// Arrange

			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			PlayPlaylistStartingFromSongEventArgs receivedEvent = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => receivedEvent = e);

			// Act

			target.PlayFromSong();

			// Assert

			Assert.IsNull(receivedEvent);

			// Avoiding uncovered lambda code (e => receivedEvent = e)
			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs(null));
		}

		[Test]
		public void OnPlaylistChanged_SendsPlaylistChangedEvent()
		{
			// Arrange

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			PlaylistChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			target.InvokeOnPlaylistChanged();

			// Assert

			Assert.IsNotNull(receivedEvent);
		}

		[Test]
		public void NavigateToSongDiscCommand_IfSomeSongIsSelected_SendsNavigateLibraryExplorerToDiscEventForDiscOfThisSong()
		{
			// Arrange

			var currentDisc = new Disc();
			var selectedDisc = new Disc();

			Song currentSong = new Song { Disc = currentDisc };
			Song selectedSong = new Song { Disc = selectedDisc };

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { currentSong, selectedSong });
			target.SwitchToSong(currentSong);
			target.SelectedSongItem = new SongListItem(selectedSong);

			NavigateLibraryExplorerToDiscEventArgs receivedEvent = null;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => receivedEvent = e);

			// Act

			target.NavigateToSongDisc();

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(selectedDisc, receivedEvent.Disc);
		}

		[Test]
		public void NavigateToSongDiscCommand_IfNoSongIsSelected_SendsNavigateLibraryExplorerToDiscEventForDiscOfCurrentSong()
		{
			// Arrange

			var currentDisc = new Disc();
			Song currentSong = new Song { Disc = currentDisc };

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song(), currentSong });
			target.SwitchToSong(currentSong);

			NavigateLibraryExplorerToDiscEventArgs receivedEvent = null;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => receivedEvent = e);

			// Act

			target.NavigateToSongDisc();

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(currentDisc, receivedEvent.Disc);
		}

		[Test]
		public void NavigateToSongDiscCommand_IfThereIsNoSelectedOrCurrentSong_DoesNothing()
		{
			// Arrange

			var target = new SongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song() });

			bool receivedEvent = false;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => receivedEvent = true);

			// Act

			target.NavigateToSongDisc();

			// Assert

			Assert.IsFalse(receivedEvent);

			// Avoiding uncovered lambda code (e => receivedEvent = true)
			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(null));
		}
	}
}
