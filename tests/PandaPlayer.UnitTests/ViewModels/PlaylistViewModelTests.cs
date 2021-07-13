using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class PlaylistViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void DisplayTrackNumbersGetter_ReturnsFalse()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			// Act

			var displayTrackNumbers = target.DisplayTrackNumbers;

			// Assert

			displayTrackNumbers.Should().BeFalse();
		}

		[TestMethod]
		public async Task CurrentDisc_CurrentSongIsSet_ReturnsDiscOfThisSong()
		{
			// Arrange

			var disc1 = new DiscModel { Id = new ItemId("1") };
			var disc2 = new DiscModel { Id = new ItemId("2") };
			var disc3 = new DiscModel { Id = new ItemId("3") };

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc2,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc3,
				},
			};

			disc1.AllSongs = new[] { songs[0] };
			disc2.AllSongs = new[] { songs[1] };
			disc3.AllSongs = new[] { songs[2] };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			target.CurrentSong.Should().Be(songs[1]);

			// Act

			var currentDisc = target.CurrentDisc;

			// Assert

			currentDisc.Should().Be(disc2);
		}

		[TestMethod]
		public async Task CurrentDisc_CurrentSongIsNotSetAndAllSongsBelongToOneDisc_ReturnsThisDisc()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("1") };

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc,
				},
			};

			disc.AllSongs = songs;

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			target.CurrentSong.Should().BeNull();

			// Act

			var currentDisc = target.CurrentDisc;

			// Assert

			currentDisc.Should().Be(disc);
		}

		[TestMethod]
		public async Task CurrentDisc_CurrentSongIsNotSetAndSongsBelongToSeveralDiscs_ReturnsNull()
		{
			// Arrange

			var disc1 = new DiscModel { Id = new ItemId("1") };
			var disc2 = new DiscModel { Id = new ItemId("2") };

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc2,
				},
			};

			disc1.AllSongs = new[] { songs[0] };
			disc2.AllSongs = new[] { songs[1] };

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			target.CurrentSong.Should().BeNull();

			// Act

			var currentDisc = target.CurrentDisc;

			// Assert

			currentDisc.Should().BeNull();
		}

		[TestMethod]
		public async Task SetPlaylistSongs_ForEmptyPlaylist_SetsPlaylistSongsCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task SetPlaylistSongs_ForNonEmptyPlaylist_SetsPlaylistSongsCorrectly()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var newSongs = new[]
			{
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			await target.SetPlaylistSongs(oldSongs, CancellationToken.None);
			playlistChangedEventArgs = null;
			propertyChangedEvents.Clear();

			// Act

			await target.SetPlaylistSongs(newSongs, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(newSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(newSongs, null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task SwitchToNextSong_ForFirstCall_SetsFirstSongAsCurrent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			target.CurrentSong.Should().Be(songs[0]);
			target.SongItems[0].IsCurrentlyPlayed.Should().BeTrue();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 0);
		}

		[TestMethod]
		public async Task SwitchToNextSong_IfCurrentSongIsInListMiddle_SwitchesToNextSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			target.CurrentSong.Should().Be(songs[1]);
			target.SongItems[0].IsCurrentlyPlayed.Should().BeFalse();
			target.SongItems[1].IsCurrentlyPlayed.Should().BeTrue();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 1);
		}

		[TestMethod]
		public async Task SwitchToNextSong_IfCurrentSongIsLast_SetsCurrentSongToNull()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			target.CurrentSong.Should().BeNull();
			target.SongItems[1].IsCurrentlyPlayed.Should().BeFalse();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, null);
		}

		[TestMethod]
		public async Task PlayFromSongCommand_NoSongIsSelected_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlayPlaylistStartingFromSongEventArgs playPlaylistEventArgs = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => e.RegisterEvent(ref playPlaylistEventArgs));

			// Act

			target.PlayFromSongCommand.Execute(null);

			// Assert

			target.CurrentSong.Should().BeNull();

			playPlaylistEventArgs.Should().BeNull();
		}

		[TestMethod]
		public async Task PlayFromSongCommand_SomeSongIsSelected_SetsThisSongAsCurrentAndSendsPlayPlaylistStartingFromSongEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			target.SelectedSongItem = target.SongItems[1];

			PlayPlaylistStartingFromSongEventArgs playPlaylistEventArgs = null;
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, e => e.RegisterEvent(ref playPlaylistEventArgs));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			target.PlayFromSongCommand.Execute(null);

			// Assert

			target.CurrentSong.Should().Be(songs[1]);
			target.SongItems[1].IsCurrentlyPlayed.Should().BeTrue();

			playPlaylistEventArgs.Should().NotBeNull();
			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 1);
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylistCommand_NoSongsAreSelected_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.RemoveSongsFromPlaylistCommand.Execute(null);

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.Should().BeNull();
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylistCommand_SomeSongsAreSelected_RemovesSelectedSongsFromPlaylist()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
				target.SongItems[4],
			};

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.RemoveSongsFromPlaylistCommand.Execute(null);

			// Assert

			var expectedLeftSongs = new[]
			{
				songs[1],
				songs[3],
			};

			target.Songs.Should().BeEquivalentTo(expectedLeftSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedLeftSongs, null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylistCommand_CurrentSongIsRemoved_SetsCurrentSongToNull()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[1],
				target.SongItems[2],
				target.SongItems[3],
			};

			target.CurrentSong.Should().Be(songs[2]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			target.RemoveSongsFromPlaylistCommand.Execute(null);

			// Assert

			target.CurrentSong.Should().BeNull();
			target.SongItems.Should().OnlyContain(x => !x.IsCurrentlyPlayed);

			playlistChangedEventArgs.CurrentSong.Should().BeNull();
			playlistChangedEventArgs.CurrentSongIndex.Should().BeNull();
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylistCommand_CurrentSongIsNotRemoved_LeavesCurrentSongUnchanged()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[1],
				target.SongItems[3],
			};

			target.CurrentSong.Should().Be(songs[2]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			target.RemoveSongsFromPlaylistCommand.Execute(null);

			// Assert

			target.CurrentSong.Should().Be(songs[2]);
			target.SongItems.Where(x => x.IsCurrentlyPlayed).Should().HaveCount(1);
			target.SongItems[1].IsCurrentlyPlayed.Should().BeTrue();

			playlistChangedEventArgs.CurrentSong.Should().Be(songs[2]);
			playlistChangedEventArgs.CurrentSongIndex.Should().Be(1);
		}

		[TestMethod]
		public void ClearPlaylistCommand_ForEmptyPlaylist_DoesNothing()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.ClearPlaylistCommand.Execute(null);

			// Assert

			target.Songs.Should().BeEmpty();
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.Should().BeNull();

			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ClearPlaylistCommand_ForNonEmptyPlaylist_ClearsPlaylistCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.ClearPlaylistCommand.Execute(null);

			// Assert

			target.Songs.Should().BeEmpty();
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(Array.Empty<SongModel>(), null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task NavigateToSongDiscCommand_SomeSongIsSelected_NavigatesToDiscForSelectedSong()
		{
			// Arrange

			var disc1 = new DiscModel();
			var disc2 = new DiscModel();

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc2,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			await target.SwitchToNextSong(CancellationToken.None);
			target.SelectedSongItem = target.SongItems[2];
			target.CurrentSong.Should().Be(songs[0]);

			NavigateLibraryExplorerToDiscEventArgs navigateToDiscEventArgs = null;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => e.RegisterEvent(ref navigateToDiscEventArgs));

			// Act

			target.NavigateToSongDiscCommand.Execute(null);

			// Assert

			navigateToDiscEventArgs.Disc.Should().Be(disc2);
		}

		[TestMethod]
		public async Task NavigateToSongDiscCommand_NoSongIsSelected_NavigatesToDiscForCurrentSong()
		{
			// Arrange

			var disc1 = new DiscModel();
			var disc2 = new DiscModel();

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc2,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc1,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[1]);

			NavigateLibraryExplorerToDiscEventArgs navigateToDiscEventArgs = null;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => e.RegisterEvent(ref navigateToDiscEventArgs));

			// Act

			target.NavigateToSongDiscCommand.Execute(null);

			// Assert

			navigateToDiscEventArgs.Disc.Should().Be(disc2);
		}

		[TestMethod]
		public async Task NavigateToSongDiscCommand_NoSongIsSelectedOrPlaying_DoesNotNavigateToAnyDisc()
		{
			// Arrange

			var disc1 = new DiscModel();
			var disc2 = new DiscModel();

			var songs = new[]
			{
				new SongModel
				{
					Id = new ItemId("0"),
					Disc = disc1,
				},
				new SongModel
				{
					Id = new ItemId("1"),
					Disc = disc2,
				},
				new SongModel
				{
					Id = new ItemId("2"),
					Disc = disc1,
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			NavigateLibraryExplorerToDiscEventArgs navigateToDiscEventArgs = null;
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => e.RegisterEvent(ref navigateToDiscEventArgs));

			// Act

			target.NavigateToSongDiscCommand.Execute(null);

			// Assert

			navigateToDiscEventArgs.Should().BeNull();
		}

		// There are 2 ways to add songs to PlaylistViewModel: via command (e.g. PlaySongsNextCommand) or via event (e.g. AddingSongsToPlaylistNextEventArgs).
		// We test all possible cases via event handlers. For each command handlers we have only single basic test.
		// See also comment in PlaylistViewModel constructor for command and event handlers.
		[TestMethod]
		public async Task AddingSongsToPlaylistNextEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsSet_DoesNotChangeCurrentSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(songsToAdd));

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[1],
				songsToAdd[0],
				songsToAdd[1],
				songs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songs[1]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 1);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistNextEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_SetsCurrentSongToFirstAddedSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(songsToAdd));

			// Assert

			var expectedSongs = new[]
			{
				songsToAdd[0],
				songsToAdd[1],
				songs[0],
				songs[1],
				songs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public void AddingSongsToPlaylistNextEventHandler_ForEmptyPlaylist_FillsPlaylistWithPassedSongs()
		{
			// Arrange

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(songsToAdd));

			// Assert

			target.Songs.Should().BeEquivalentTo(songsToAdd, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(songsToAdd, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistNextEventHandler_IfAddedSongListIsEmpty_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(Enumerable.Empty<SongModel>()));

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(null);

			playlistChangedEventArgs.Should().BeNull();
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public async Task PlaySongsNextCommand_SomeSongsAreSelected_AddsSelectedSongsToPlaylist()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			AddingSongsToPlaylistNextEventArgs addingSongsToPlaylistNextEventArgs = null;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistNextEventArgs));

			// Act

			target.PlaySongsNextCommand.Execute(null);

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[2],
				songs[0],
				songs[1],
				songs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(expectedSongs[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();

			addingSongsToPlaylistNextEventArgs.Should().BeNull();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistLastEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsSet_DoesNotChangeCurrentSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(songsToAdd));

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[1],
				songs[2],
				songsToAdd[0],
				songsToAdd[1],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songs[1]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 1);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistLastEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_SetsCurrentSongToFirstAddedSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("3") },
				new SongModel { Id = new ItemId("4") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(songsToAdd));

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[1],
				songs[2],
				songsToAdd[0],
				songsToAdd[1],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 3);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public void AddingSongsToPlaylistLastEventHandler_ForEmptyPlaylist_FillsPlaylistWithPassedSongs()
		{
			// Arrange

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(songsToAdd));

			// Assert

			target.Songs.Should().BeEquivalentTo(songsToAdd, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(songsToAdd, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistLastEventHandler_IfAddedSongListIsEmpty_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(Enumerable.Empty<SongModel>()));

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(null);

			playlistChangedEventArgs.Should().BeNull();
			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public async Task PlaySongsLastCommand_SomeSongsAreSelected_AddsSelectedSongsToPlaylist()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistLastEventArgs = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistLastEventArgs));

			// Act

			target.PlaySongsLastCommand.Execute(null);

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[1],
				songs[2],
				songs[0],
				songs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(expectedSongs[3]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 3);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();

			addingSongsToPlaylistLastEventArgs.Should().BeNull();
		}
	}
}
