using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.Player;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels.Player
{
	internal sealed class SongPlaylistStub : SongPlaylistViewModel
	{
		public SongPlaylistStub(IEnumerable<Song> songs) :
			base(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>())
		{
			SetSongs(songs);
		}

		public SongPlaylistStub(Song song) :
			this(Enumerable.Repeat(song, 1))
		{
		}
	}

	[TestFixture]
	public class MusicPlayerViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicPlayerViewModel(null, Substitute.For<ISongPlaylistViewModel>(),
				Substitute.For<IAudioPlayer>(), Substitute.For<ISongPlaybacksRegistrator>()));
		}

		[Test]
		public void Constructor_IfPlaylistArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), null,
				Substitute.For<IAudioPlayer>(), Substitute.For<ISongPlaybacksRegistrator>()));
		}

		[Test]
		public void Constructor_IfAudioPlayerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<ISongPlaylistViewModel>(),
				null, Substitute.For<ISongPlaybacksRegistrator>()));
		}

		[Test]
		public void Constructor_IfPlaybacksRegistratorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<ISongPlaylistViewModel>(),
				Substitute.For<IAudioPlayer>(), null));
		}

		[Test]
		public void Play_IfNotStartedAndPlaylistEndIsReached_ReturnsWithNoAction()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub((Song)null);
			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			//	Act

			target.Play().Wait();

			//	Assert

			Assert.IsFalse(target.IsPlaying);
			Assert.IsNull(target.CurrentSong);
			audioPlayerMock.DidNotReceive().Play();
		}

		[Test]
		public void Play_IfNotStartedAndPlaylistEndIsNotReached_StartsPlaybackOfCurrentSongInPlaylist()
		{
			//	Arrange

			var song = new Song();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetSongFile(song).Returns("SomeSong.mp3");

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(song);
			songPlaylistStub.SwitchToNextSong();
			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(musicLibraryStub, songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			//	Act

			target.Play().Wait();

			//	Assert

			Assert.IsTrue(target.IsPlaying);
			Assert.AreSame(song, target.CurrentSong);
			Received.InOrder(() => {
				audioPlayerMock.Received(1).SetCurrentSongFile("SomeSong.mp3");
				audioPlayerMock.Received(1).Play();
			});
		}

		[Test]
		public void Play_IfPaused_ResumesPlayback()
		{
			//	Arrange

			var song = new Song();

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(song);
			songPlaylistStub.SwitchToNextSong();
			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();
			target.Pause();
			audioPlayerMock.ClearReceivedCalls();

			//	Act

			target.Play().Wait();

			//	Assert

			Assert.IsTrue(target.IsPlaying);
			Assert.AreSame(song, target.CurrentSong);
			audioPlayerMock.Received(1).Play();
		}

		[Test]
		public void Play_WhenSwitchingToNewSong_RegistersPlaybackStart()
		{
			//	Arrange

			var song = new Song();

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(song);
			songPlaylistStub.SwitchToNextSong();

			ISongPlaybacksRegistrator songPlaybacksRegistrator = Substitute.For<ISongPlaybacksRegistrator>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, Substitute.For<IAudioPlayer>(), songPlaybacksRegistrator);

			//	Act

			target.Play().Wait();

			//	Assert

			songPlaybacksRegistrator.Received(1).RegisterPlaybackStart(song);
		}

		[Test]
		public void Play_WhenSongPlaybackFinishes_RegisterPlaybackFinish()
		{
			//	Arrange

			var song = new Song();

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(song);
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerStub = Substitute.For<IAudioPlayer>();

			ISongPlaybacksRegistrator songPlaybacksRegistrator = Substitute.For<ISongPlaybacksRegistrator>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerStub, songPlaybacksRegistrator);

			target.Play().Wait();

			//	Act

			audioPlayerStub.SongMediaFinished += Raise.EventWith(new SongMediaFinishedEventArgs());

			//	Assert

			songPlaybacksRegistrator.Received(1).RegisterPlaybackFinish(song);
		}

		[Test]
		public void Play_WhenSongPlaybackFinishes_SwitchesPlaylistToNextSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			ISongPlaylistViewModel songPlaylistMock = new SongPlaylistStub(new[] { song1, song2 });
			songPlaylistMock.SwitchToNextSong();

			IAudioPlayer audioPlayerStub = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistMock, audioPlayerStub, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();

			//	Act

			audioPlayerStub.SongMediaFinished += Raise.EventWith(new SongMediaFinishedEventArgs());

			//	Assert

			Assert.AreSame(song2, songPlaylistMock.CurrentSong);
		}

		[Test]
		public void Play_WhenSongPlaybackFinishes_StartsPlaybackOfNextSongInPlaylist()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetSongFile(song2).Returns("SomeSong2.mp3");

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new[] { song1, song2 });
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(musicLibraryStub, songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();
			audioPlayerMock.ClearReceivedCalls();

			//	Act

			audioPlayerMock.SongMediaFinished += Raise.EventWith(new SongMediaFinishedEventArgs());

			//	Assert

			Assert.IsTrue(target.IsPlaying);
			Assert.AreSame(song2, target.CurrentSong);
			Received.InOrder(() => {
				audioPlayerMock.Received(1).SetCurrentSongFile("SomeSong2.mp3");
				audioPlayerMock.Received(1).Play();
			});
		}

		[Test]
		public void Play_WhenPlaylistFinishIsReached_StopsPlaying()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new Song());
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();
			audioPlayerMock.ClearReceivedCalls();

			//	Act

			audioPlayerMock.SongMediaFinished += Raise.EventWith(new SongMediaFinishedEventArgs());

			//	Assert

			Assert.IsFalse(target.IsPlaying);
			Assert.IsNull(target.CurrentSong);
			audioPlayerMock.DidNotReceive().Play();
		}

		[Test]
		public void Play_WhenPlaylistFinishIsReached_SendsPlaylistFinishedEvent()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new Song());
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerStub = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerStub, Substitute.For<ISongPlaybacksRegistrator>());

			bool receivedEvent = false;
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, e => receivedEvent = true);

			target.Play().Wait();

			//	Act

			audioPlayerStub.SongMediaFinished += Raise.EventWith(new SongMediaFinishedEventArgs());

			//	Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void Pause_WhenPlaying_PausesPlayback()
		{
			//	Arrange

			var song = new Song();

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(song);
			songPlaylistStub.SwitchToNextSong();
			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();
			audioPlayerMock.ClearReceivedCalls();

			//	Act

			target.Pause();

			//	Assert

			Assert.IsFalse(target.IsPlaying);
			Assert.AreSame(song, target.CurrentSong);
			audioPlayerMock.Received(1).Pause();
		}

		[Test]
		public void Stop_IfSomeSongIsPlayed_StopsPlayback()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new[] { new Song() });
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();

			//	Act

			target.Stop();

			//	Assert

			Assert.IsFalse(target.IsPlaying);
			audioPlayerMock.Received(1).Stop();
		}

		[Test]
		public void Stop_IfSomeSongIsPlayed_SetsCurrentSongToNull()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new[] { new Song() });
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			target.Play().Wait();

			//	Act

			target.Stop();

			//	Assert

			Assert.IsNull(target.CurrentSong);
		}

		[Test]
		public void Stop_IfSomeSongIsPlayed_DoesNotRegisterPlaybackFinish()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new[] { new Song() });
			songPlaylistStub.SwitchToNextSong();

			ISongPlaybacksRegistrator songPlaybacksRegistratorMock = Substitute.For<ISongPlaybacksRegistrator>();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, songPlaybacksRegistratorMock);

			target.Play().Wait();

			//	Act

			target.Stop();

			//	Assert

			songPlaybacksRegistratorMock.DidNotReceive().RegisterPlaybackFinish(Arg.Any<Song>());
		}

		[Test]
		public void Stop_IfNoSongIsPlayed_DoesNothing()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistStub = new SongPlaylistStub(new[] { new Song() });
			songPlaylistStub.SwitchToNextSong();

			IAudioPlayer audioPlayerMock = Substitute.For<IAudioPlayer>();
			var target = new MusicPlayerViewModel(Substitute.For<IMusicLibrary>(), songPlaylistStub, audioPlayerMock, Substitute.For<ISongPlaybacksRegistrator>());

			//	Act

			target.Stop();

			//	Assert

			audioPlayerMock.DidNotReceive().Stop();
		}
	}
}
