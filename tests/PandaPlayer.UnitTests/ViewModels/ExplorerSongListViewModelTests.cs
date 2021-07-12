using System;
using System.Collections.Generic;
using System.Threading;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels;

namespace PandaPlayer.UnitTests.ViewModels
{
	[TestClass]
	public class ExplorerSongListViewModelTests
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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

			target.SetSongs(songs);

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			mocker.GetMock<IWindowService>().Verify(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>()), Times.Never);
			mocker.GetMock<ISongsService>().Verify(x => x.DeleteSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public void DeleteSongsFromDiscCommand_MessageBoxResultIsNotYes_DoesNothing()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();
			mocker.GetMock<IWindowService>().Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>()))
				.Returns(ShowMessageBoxResult.No);

			target.SetSongs(songs);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			mocker.GetMock<ISongsService>().Verify(x => x.DeleteSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public void DeleteSongsFromDiscCommand_MessageBoxResultIsYes_DeletesSongsViaSongsService()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("0") },
				new SongModel { Id = new ItemId("1") },
				new SongModel { Id = new ItemId("2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();
			mocker.GetMock<IWindowService>().Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>()))
				.Returns(ShowMessageBoxResult.Yes);

			target.SetSongs(songs);

			target.SelectedSongItems = new[]
			{
				target.SongItems[0],
				target.SongItems[2],
			};

			// Act

			target.DeleteSongsFromDiscCommand.Execute(null);

			// Assert

			mocker.GetMock<ISongsService>().Verify(x => x.DeleteSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
			mocker.GetMock<ISongsService>().Verify(x => x.DeleteSong(songs[0], It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<ISongsService>().Verify(x => x.DeleteSong(songs[2], It.IsAny<CancellationToken>()), Times.Once);
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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();
			mocker.GetMock<IWindowService>().Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ShowMessageBoxButton>(), It.IsAny<ShowMessageBoxIcon>()))
				.Returns(ShowMessageBoxResult.Yes);

			mocker.GetMock<ISongsService>().Setup(x => x.DeleteSong(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()))
				.Callback<SongModel, CancellationToken>((song, _) => song.DeleteDate = new DateTime(2021, 07, 11));

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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

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
			var target = mocker.CreateInstance<ExplorerSongListViewModel>();

			AddingSongsToPlaylistLastEventArgs addingSongsToPlaylistEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => e.RegisterEvent(ref addingSongsToPlaylistEvent));

			target.SetSongs(songs);

			// Act

			target.PlaySongsLastCommand.Execute(null);

			// Assert

			addingSongsToPlaylistEvent.Should().BeNull();
			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}
	}
}
