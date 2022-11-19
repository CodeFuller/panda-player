using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.UnitTests.ViewModels.Player
{
	[TestClass]
	public class PlaylistPlayerViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public async Task ReversePlaying_IfNoSongIsPlaying_StartsPlayingCurrentPlaylistSong()
		{
			// Arrange

			var song = new SongModel();

			var mocker = new AutoMocker();
			mocker.GetMock<IPlaylistViewModel>()
				.Setup(x => x.CurrentSong).Returns(song);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			// Act

			await target.ReversePlaying(CancellationToken.None);

			// Assert

			var songPlayerMock = mocker.GetMock<ISongPlayerViewModel>();
			songPlayerMock.Verify(x => x.Play(song, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ReversePlaying_IfCurrentPlaylistSongIsNotSet_DoesNotStartPlaying()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IPlaylistViewModel>()
				.Setup(x => x.CurrentSong).Returns<SongModel>(null);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			// Act

			await target.ReversePlaying(CancellationToken.None);

			// Assert

			var songPlayerMock = mocker.GetMock<ISongPlayerViewModel>();
			songPlayerMock.Verify(x => x.Play(It.IsAny<SongModel>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task ReversePlaying_IfSongIsPlaying_ReversesPlaying()
		{
			// Arrange

			var songPlayerMock = new Mock<ISongPlayerViewModel>();

			var mocker = new AutoMocker();
			mocker.Use(songPlayerMock);

			mocker.GetMock<IPlaylistViewModel>()
				.Setup(x => x.CurrentSong).Returns(new SongModel());

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			await target.ReversePlaying(CancellationToken.None);
			songPlayerMock.Invocations.Clear();

			// Act

			await target.ReversePlaying(CancellationToken.None);

			// Assert

			songPlayerMock.Verify(x => x.ReversePlaying(), Times.Once);
		}

		[TestMethod]
		public void PlaySongsListEventHandler_IfNoSongIsPlaying_StartsPlayingCurrentPlaylistSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("Song 1") },
				new SongModel { Id = new ItemId("Song 2") },
				new SongModel { Id = new ItemId("Song 3") },
			};

			var playlistMock = new Mock<IPlaylistViewModel>();
			playlistMock.Setup(x => x.CurrentSong).Returns(songs[1]);

			var mocker = new AutoMocker();
			mocker.Use(playlistMock);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(songs));

			// Assert

			var songPlayerMock = mocker.GetMock<ISongPlayerViewModel>();

			songPlayerMock.Verify(x => x.StopPlaying(), Times.Never);
			playlistMock.Verify(x => x.SetPlaylistSongs(songs, It.IsAny<CancellationToken>()), Times.Once);
			songPlayerMock.Verify(x => x.Play(songs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PlaySongsListEventHandler_IfSomeSongIsPlaying_StartsPlayingCurrentPlaylistSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("Song 1") },
				new SongModel { Id = new ItemId("Song 2") },
				new SongModel { Id = new ItemId("Song 3") },
			};

			var playlistMock = new Mock<IPlaylistViewModel>();
			playlistMock.Setup(x => x.CurrentSong).Returns(songs[1]);

			var songPlayerMock = new Mock<ISongPlayerViewModel>();

			var mocker = new AutoMocker();
			mocker.Use(playlistMock);
			mocker.Use(songPlayerMock);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			await target.ReversePlaying(CancellationToken.None);
			playlistMock.Invocations.Clear();
			songPlayerMock.Invocations.Clear();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(songs));

			// Assert

			songPlayerMock.Verify(x => x.StopPlaying(), Times.Once);
			playlistMock.Verify(x => x.SetPlaylistSongs(songs, It.IsAny<CancellationToken>()), Times.Once);
			songPlayerMock.Verify(x => x.Play(songs[1], It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void PlayPlaylistStartingFromSongEventHandler_IfNoSongIsPlaying_StartsPlayingCurrentPlaylistSong()
		{
			// Arrange

			var currentSong = new SongModel();

			var playlistMock = new Mock<IPlaylistViewModel>();
			playlistMock.Setup(x => x.CurrentSong).Returns(currentSong);

			var mocker = new AutoMocker();
			mocker.Use(playlistMock);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			// Act

			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs());

			// Assert

			var songPlayerMock = mocker.GetMock<ISongPlayerViewModel>();

			songPlayerMock.Verify(x => x.StopPlaying(), Times.Never);
			songPlayerMock.Verify(x => x.Play(currentSong, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PlayPlaylistStartingFromSongEventHandler_IfSomeSongIsPlaying_StartsPlayingCurrentPlaylistSong()
		{
			// Arrange

			var currentSong = new SongModel();

			var playlistMock = new Mock<IPlaylistViewModel>();
			playlistMock.Setup(x => x.CurrentSong).Returns(currentSong);

			var songPlayerMock = new Mock<ISongPlayerViewModel>();

			var mocker = new AutoMocker();
			mocker.Use(playlistMock);
			mocker.Use(songPlayerMock);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			await target.ReversePlaying(CancellationToken.None);
			playlistMock.Invocations.Clear();
			songPlayerMock.Invocations.Clear();

			// Act

			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs());

			// Assert

			songPlayerMock.Verify(x => x.StopPlaying(), Times.Once);
			songPlayerMock.Verify(x => x.Play(currentSong, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SongPlaybackFinishedEventHandler_IfPlaylistHasMoreSongs_StartsPlayingNextSong()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("Song 1") },
				new SongModel { Id = new ItemId("Song 2") },
			};

			var currentSongIndex = 0;

			var playlistStub = new Mock<IPlaylistViewModel>();
			playlistStub.Setup(x => x.Songs).Returns(songs);
			playlistStub.Setup(x => x.CurrentSong).Returns(() => songs[currentSongIndex]);
			playlistStub.Setup(x => x.SwitchToNextSong(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => currentSongIndex++);

			var songPlayerMock = new Mock<ISongPlayerViewModel>();

			var mocker = new AutoMocker();
			mocker.Use(playlistStub);
			mocker.Use(songPlayerMock);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			await target.ReversePlaying(CancellationToken.None);

			songPlayerMock.Invocations.Clear();

			PlaylistFinishedEventArgs playlistFinishedEvent = null;
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => e.RegisterEvent(ref playlistFinishedEvent));

			// Act

			Messenger.Default.Send(new SongPlaybackFinishedEventArgs());

			// Assert

			songPlayerMock.Verify(x => x.Play(songs[1], It.IsAny<CancellationToken>()), Times.Once);

			playlistFinishedEvent.Should().BeNull();
		}

		[TestMethod]
		public async Task SongPlaybackFinishedEventHandler_IfPlaylistHasNoMoreSongs_SendsPlaylistFinishedEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("Song 1") },
				new SongModel { Id = new ItemId("Song 2") },
			};

			var currentSong = songs.Last();

			var playlistStub = new Mock<IPlaylistViewModel>();
			playlistStub.Setup(x => x.Songs).Returns(songs);
			playlistStub.Setup(x => x.CurrentSong).Returns(() => currentSong);
			playlistStub.Setup(x => x.SwitchToNextSong(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => currentSong = null);

			var mocker = new AutoMocker();
			mocker.Use(playlistStub);

			var target = mocker.CreateInstance<PlaylistPlayerViewModel>();

			await target.ReversePlaying(CancellationToken.None);

			PlaylistFinishedEventArgs playlistFinishedEvent = null;
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => e.RegisterEvent(ref playlistFinishedEvent));

			// Act

			Messenger.Default.Send(new SongPlaybackFinishedEventArgs());

			// Assert

			playlistFinishedEvent.Should().NotBeNull();
			playlistFinishedEvent.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
		}
	}
}
