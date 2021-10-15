using System;
using System.Collections.Generic;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class DiscSongListViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void DisplayTrackNumbersGetter_ReturnsTrue()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			// Act

			var displayTrackNumbers = target.DisplayTrackNumbers;

			// Assert

			displayTrackNumbers.Should().BeTrue();
		}

		[TestMethod]
		public void DeleteSongsFromDiscCommand_NoSongsAreSelected_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowDeleteDiscSongsView(It.IsAny<IReadOnlyCollection<SongModel>>()), Times.Never);
		}

		[TestMethod]
		public void DeleteSongsFromDiscCommand_SomeSongsAreSelected_InvokesDeleteDiscSongsViewForSelectedSongs()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(songs);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			mocker.GetMock<IViewNavigator>().Verify(x => x.ShowDeleteDiscSongsView(new[] { songs[0], songs[2] }), Times.Once);
		}

		[TestMethod]
		public void DeleteSongsFromDiscCommand_WhenSongsAreDeleted_SongItemsAreRemovedFromList()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
				new SongModel { Id = new ItemId("3") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			mocker.GetMock<IViewNavigator>().Setup(x => x.ShowDeleteDiscSongsView(It.IsAny<IReadOnlyCollection<SongModel>>()))
				.Callback<IReadOnlyCollection<SongModel>>(deletedSongs =>
				{
					foreach (var song in deletedSongs)
					{
						song.DeleteDate = new DateTime(2021, 07, 11);
					}
				});

			target.SetSongs(songs);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			var expectedSongs = new[]
			{
				songs[1],
				songs[3],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void PlaySongsNextCommand_SomeSongsAreSelected_SendsAddingSongsToPlaylistNextEventWithSelectedSongs()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			AddingSongsToPlaylistNextEventArgs addingSongsToPlaylistEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistEvent));

			target.SetSongs(songs);
			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.PlaySongsNextCommand.Execute(null);

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[2],
			};

			addingSongsToPlaylistEvent.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void PlaySongsNextCommand_NoSongsAreSelected_DoesNotSendAddingSongsToPlaylistNextEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			AddingSongsToPlaylistNextEventArgs addingSongsToPlaylistEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistEvent));

			target.SetSongs(songs);

			// Act

			target.PlaySongsNextCommand.Execute(null);

			// Assert

			addingSongsToPlaylistEvent.Should().BeNull();
			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void PlaySongsLastCommand_SomeSongsAreSelected_SendsAddingSongsToPlaylistLastEventWithSelectedSongs()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistEvent));

			target.SetSongs(songs);
			target.SelectedSongItems = new List<SongListItem>
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.PlaySongsLastCommand.Execute(null);

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[2],
			};

			addingSongsToPlaylistEvent.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void PlaySongsLastCommand_NoSongsAreSelected_DoesNotSendAddingSongsToPlaylistLastEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistEvent));

			target.SetSongs(songs);

			// Act

			target.PlaySongsLastCommand.Execute(null);

			// Assert

			addingSongsToPlaylistEvent.Should().BeNull();
			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_IfDiscIsNotNull_FillsListWithActiveDiscSongs()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("Old 0") },
				new SongModel { Id = new ItemId("Old 1") },
			};

			var newSongs = new[]
			{
				new SongModel { Id = new ItemId("New 0") },
				new SongModel
				{
					Id = new ItemId("New 1"),
					DeleteDate = new DateTime(2021, 07, 25),
				},
				new SongModel { Id = new ItemId("New 2") },
			};

			var newDisc = new DiscModel
			{
				AllSongs = newSongs,
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(oldSongs);

			// Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(newDisc));

			// Assert

			var expectedSongs = new[]
			{
				newSongs[0],
				newSongs[2],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void LibraryExplorerDiscChangedEventHandler_IfDiscIsNull_ClearsSongList()
		{
			// Arrange

			var oldSongs = new[]
			{
				new SongModel { Id = new ItemId("Old 0") },
				new SongModel { Id = new ItemId("Old 1") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscSongListViewModel>();

			target.SetSongs(oldSongs);

			// Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(null));

			// Assert

			target.Songs.Should().BeEmpty();
		}
	}
}
