using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class PlaylistViewModelTests
	{
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
				new SongModel { Id = new ItemId("0") }.AddToDisc(disc1),
				new SongModel { Id = new ItemId("1") }.AddToDisc(disc2),
				new SongModel { Id = new ItemId("2") }.AddToDisc(disc3),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
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
				new SongModel { Id = new ItemId("0") }.AddToDisc(disc),
				new SongModel { Id = new ItemId("1") }.AddToDisc(disc),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
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

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") }.AddToDisc(new DiscModel { Id = new ItemId("1") }),
				new SongModel { Id = new ItemId("1") }.AddToDisc(new DiscModel { Id = new ItemId("2") }),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			target.CurrentSong.Should().BeNull();

			// Act

			var currentDisc = target.CurrentDisc;

			// Assert

			currentDisc.Should().BeNull();
		}

		[TestMethod]
		public void ContextMenuItems_ForEmptyPlaylist_ReturnsEmptyCollection()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			menuItems.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ContextMenuItems_IfNoSongsSelected_ReturnsCorrectMenuItems()
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

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			var expectedMenuItems = new[]
			{
				new CommandMenuItem(() => { }) { Header = "Clear Playlist", IconKind = PackIconKind.PlaylistRemove },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task ContextMenuItems_IfSomeSongsSelected_ReturnsCorrectMenuItems()
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

			var selectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			target.SelectedSongItems = selectedSongItems;
			target.SelectedSongItem = selectedSongItems[0];

			// Act

			var menuItems = target.ContextMenuItems;

			// Assert

			var expectedMenuItems = new BasicMenuItem[]
			{
				new CommandMenuItem(() => { }) { Header = "Play From This Song", IconKind = PackIconKind.Play },
				new CommandMenuItem(() => { }) { Header = "Play Next", IconKind = PackIconKind.PlaylistAdd },
				new CommandMenuItem(() => { }) { Header = "Play Last", IconKind = PackIconKind.PlaylistAdd },
				new CommandMenuItem(() => { }) { Header = "Remove From Playlist", IconKind = PackIconKind.PlaylistMinus },
				new CommandMenuItem(() => { }) { Header = "Clear Playlist", IconKind = PackIconKind.PlaylistRemove },
				new CommandMenuItem(() => { }) { Header = "Go To Disc", IconKind = PackIconKind.Album },
				new ExpandableMenuItem
				{
					Header = "Set Rating",
					IconKind = PackIconKind.Star,
					Items = new[]
					{
						new SetRatingMenuItem(RatingModel.R10, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R9, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R8, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R7, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R6, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R5, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R4, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R3, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R2, () => Task.CompletedTask),
						new SetRatingMenuItem(RatingModel.R1, () => Task.CompletedTask),
					},
				},
				new CommandMenuItem(() => { }) { Header = "Properties", IconKind = PackIconKind.Pencil },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().RespectingRuntimeTypes());
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songs[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 0);
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(oldSongs, CancellationToken.None);
			target.CurrentSong.Should().Be(oldSongs[0]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.SetPlaylistSongs(newSongs, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(newSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(newSongs[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(newSongs, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task SetPlaylistSongs_ForEmptySongList_ClearsCurrentPlaylist()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(oldSongs, CancellationToken.None);
			target.CurrentSong.Should().Be(oldSongs[0]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.SetPlaylistSongs(Enumerable.Empty<SongModel>(), CancellationToken.None);

			// Assert

			target.Songs.Should().BeEmpty();
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(Array.Empty<SongModel>(), null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task SwitchToNextSong_CurrentSongIsNotSet_ThrowsInvalidOperationException()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			Func<Task> call = () => target.SwitchToNextSong(CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>();
		}

		[TestMethod]
		public async Task SwitchToNextSong_IfCurrentSongIsInListMiddle_SwitchesToNextSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[1]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			target.CurrentSong.Should().Be(songs[2]);
			target.SongItems[1].IsCurrentlyPlayed.Should().BeFalse();
			target.SongItems[2].IsCurrentlyPlayed.Should().BeTrue();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 2);
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs.Last());

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			target.CurrentSong.Should().BeNull();
			target.SongItems[1].IsCurrentlyPlayed.Should().BeFalse();

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, null);
		}

		[TestMethod]
		public async Task PlayFromSong_ForSongFromTheList_SetsThisSongAsCurrentAndSendsPlayPlaylistStartingFromSongEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlayPlaylistStartingFromSongEventArgs playPlaylistEventArgs = null;
			messenger.Register<PlayPlaylistStartingFromSongEventArgs>(this, (_, e) => e.RegisterEvent(ref playPlaylistEventArgs));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			messenger.Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.PlayFromSong(target.SongItems[1], CancellationToken.None);

			// Assert

			target.CurrentSong.Should().Be(songs[1]);
			target.SongItems[1].IsCurrentlyPlayed.Should().BeTrue();

			playPlaylistEventArgs.Should().NotBeNull();
			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 1);
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylist_ForSongsFromTheList_RemovesSongsFromPlaylist()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.RemoveSongsFromPlaylist(new[] { target.SongItems[0], target.SongItems[2], target.SongItems[4] }, CancellationToken.None);

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
		public async Task RemoveSongsFromPlaylist_CurrentSongIsRemoved_SetsCurrentSongToNull()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[2]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.RemoveSongsFromPlaylist(new[] { target.SongItems[1], target.SongItems[2], target.SongItems[3] }, CancellationToken.None);

			// Assert

			target.CurrentSong.Should().BeNull();
			target.SongItems.Should().OnlyContain(x => !x.IsCurrentlyPlayed);

			playlistChangedEventArgs.CurrentSong.Should().BeNull();
			playlistChangedEventArgs.CurrentSongIndex.Should().BeNull();
		}

		[TestMethod]
		public async Task RemoveSongsFromPlaylist_CurrentSongIsNotRemoved_LeavesCurrentSongUnchanged()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[2]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.RemoveSongsFromPlaylist(new[] { target.SongItems[1], target.SongItems[3] }, CancellationToken.None);

			// Assert

			target.CurrentSong.Should().Be(songs[2]);
			target.SongItems.Where(x => x.IsCurrentlyPlayed).Should().HaveCount(1);
			target.SongItems[1].IsCurrentlyPlayed.Should().BeTrue();

			playlistChangedEventArgs.CurrentSong.Should().Be(songs[2]);
			playlistChangedEventArgs.CurrentSongIndex.Should().Be(1);
		}

		[TestMethod]
		public async Task ClearPlaylist_ForNonEmptyPlaylist_ClearsPlaylistCorrectly()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.ClearPlaylist(CancellationToken.None);

			// Assert

			target.Songs.Should().BeEmpty();
			target.CurrentSong.Should().BeNull();

			playlistChangedEventArgs.VerifyPlaylistEvent(Array.Empty<SongModel>(), null);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		// There are 2 ways to add songs to PlaylistViewModel: via context menu command (e.g. AddSongsNext) or via event (e.g. AddingSongsToPlaylistNextEventArgs).
		// We test all possible cases via direct method calls to AddSongsNext and AddSongsLast. For each event handler we have only single basic test.
		// See also comment in PlaylistViewModel constructor.
		[TestMethod]
		public async Task AddSongsNext_ForNonEmptyPlaylistWhenCurrentSongIsSet_DoesNotChangeCurrentSong()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[1]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsNext(songsToAdd, CancellationToken.None);

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
		public async Task AddSongsNext_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_SetsCurrentSongToFirstAddedSong()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsNext(songsToAdd, CancellationToken.None);

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
		public async Task AddSongsNext_ForEmptyPlaylist_FillsPlaylistWithPassedSongs()
		{
			// Arrange

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsNext(songsToAdd, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(songsToAdd, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(songsToAdd, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistNextEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_AddsSongsAtPlaylistStart()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			messenger.Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			messenger.Send(new AddingSongsToPlaylistNextEventArgs(songsToAdd));

			// Assert

			var expectedSongs = songsToAdd.Concat(songs).ToList();

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddSongsLast_ForNonEmptyPlaylistWhenCurrentSongIsSet_DoesNotChangeCurrentSong()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().Be(songs[1]);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsLast(songsToAdd, CancellationToken.None);

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
		public async Task AddSongsLast_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_SetsCurrentSongToFirstAddedSong()
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
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsLast(songsToAdd, CancellationToken.None);

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
		public async Task AddSongsLast_ForEmptyPlaylist_FillsPlaylistWithPassedSongs()
		{
			// Arrange

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
			};

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			mocker.Get<IMessenger>().Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			await target.AddSongsLast(songsToAdd, CancellationToken.None);

			// Assert

			target.Songs.Should().BeEquivalentTo(songsToAdd, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(songsToAdd, 0);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public async Task AddingSongsToPlaylistLastEventHandler_ForNonEmptyPlaylistWhenCurrentSongIsNotSet_AddsSongsAtPlaylistEnd()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
			};

			var songsToAdd = new[]
			{
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();
			var target = mocker.CreateInstance<PlaylistViewModel>();

			await target.SetPlaylistSongs(songs, CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);
			target.CurrentSong.Should().BeNull();

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			messenger.Register<PlaylistChangedEventArgs>(this, (_, e) => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			messenger.Send(new AddingSongsToPlaylistLastEventArgs(songsToAdd));

			// Assert

			var expectedSongs = songs.Concat(songsToAdd).ToList();

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songsToAdd[0]);

			playlistChangedEventArgs.VerifyPlaylistEvent(expectedSongs, 1);
			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}
	}
}
