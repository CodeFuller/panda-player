using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.UnitTests.Helpers;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.UnitTests.ViewModels.Player
{
	[TestClass]
	public class SongPlayerViewModelTests
	{
		[TestMethod]
		public void SongLengthGetter_ReturnsCurrentSongLengthFromAudioPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IAudioPlayer>()
				.Setup(x => x.SongLength).Returns(new TimeSpan(1, 2, 3));

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			var songLength = target.SongLength;

			// Assert

			songLength.Should().Be(new TimeSpan(1, 2, 3));
		}

		[TestMethod]
		public void SongPositionGetter_ReturnsCurrentSongPositionFromAudioPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IAudioPlayer>()
				.Setup(x => x.SongPosition).Returns(new TimeSpan(1, 2, 3));

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			var songPosition = target.SongPosition;

			// Assert

			songPosition.Should().Be(new TimeSpan(1, 2, 3));
		}

		[TestMethod]
		public void SongProgressGetter_IfCurrentSongLengthIsNotZero_ReturnsCurrentSongProgressInPercentages()
		{
			// Arrange

			var audioPlayerStub = new Mock<IAudioPlayer>();
			audioPlayerStub.Setup(x => x.SongPosition).Returns(new TimeSpan(0, 1, 18));
			audioPlayerStub.Setup(x => x.SongLength).Returns(new TimeSpan(0, 2, 40));

			var mocker = new AutoMocker();
			mocker.Use(audioPlayerStub);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			var songProgress = target.SongProgress;

			// Assert

			songProgress.Should().Be(48.75);
		}

		[TestMethod]
		public void SongProgressGetter_IfCurrentSongLengthIsZero_ReturnsZero()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IAudioPlayer>()
				.Setup(x => x.SongLength).Returns(TimeSpan.Zero);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			var songProgress = target.SongProgress;

			// Assert

			songProgress.Should().Be(0);
		}

		[TestMethod]
		public void SongProgressSetter_SetsCurrentSongPositionCorrectly()
		{
			// Arrange

			var audioPlayerMock = new Mock<IAudioPlayer>();
			audioPlayerMock.Setup(x => x.SongLength).Returns(new TimeSpan(0, 2, 40));

			var mocker = new AutoMocker();
			mocker.Use(audioPlayerMock);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SongProgress = 0.4875;

			// Assert

			audioPlayerMock.VerifySet(x => x.SongPosition = new TimeSpan(0, 1, 18), Times.Once);

			var expectedProperties = new[]
			{
				nameof(SongPlayerViewModel.SongProgress),
				nameof(SongPlayerViewModel.SongPosition),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void VolumeGetter_ReturnsVolumeFromAudioPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.GetMock<IAudioPlayer>()
				.Setup(x => x.Volume).Returns(0.2334);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			var volume = target.Volume;

			// Assert

			volume.Should().Be(0.2334);
		}

		[TestMethod]
		public void VolumeSetter_SetsVolumeCorrectly()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.Volume = 0.2334;

			// Assert

			var audioPlayerMock = mocker.GetMock<IAudioPlayer>();
			audioPlayerMock.VerifySet(x => x.Volume = 0.2334, Times.Once);

			var expectedProperties = new[]
			{
				nameof(SongPlayerViewModel.Volume),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public async Task Play_StartsPlayingSongInAudioPlayer()
		{
			// Arrange

			var song = new SongModel
			{
				ContentUri = new Uri("Some Uri", UriKind.Relative),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			await target.Play(song, CancellationToken.None);

			// Assert

			var audioPlayerMock = mocker.GetMock<IAudioPlayer>();
			audioPlayerMock.Verify(x => x.Open(new Uri("Some Uri", UriKind.Relative)), Times.Once);
			audioPlayerMock.Verify(x => x.Play(), Times.Once);
		}

		[TestMethod]
		public async Task Play_SetsReversePlayingKindToPause()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			await target.Play(new SongModel(), CancellationToken.None);

			// Assert

			target.ReversePlayingKind.Should().Be("Pause");
		}

		[TestMethod]
		public async Task Play_RegisterPlaybackStart()
		{
			// Arrange

			var song = new SongModel();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			// Act

			await target.Play(song, CancellationToken.None);

			// Assert

			var playbacksRegistrarMock = mocker.GetMock<ISongPlaybacksRegistrar>();
			playbacksRegistrarMock.Verify(x => x.RegisterPlaybackStart(song, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ReversePlaying_ForPlayingPlayer_PausesPlaying()
		{
			// Arrange

			var audioPlayerMock = new Mock<IAudioPlayer>();
			var playbacksRegistrarMock = new Mock<ISongPlaybacksRegistrar>();

			var mocker = new AutoMocker();
			mocker.Use(audioPlayerMock);
			mocker.Use(playbacksRegistrarMock);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			await target.Play(new SongModel(), CancellationToken.None);
			audioPlayerMock.Invocations.Clear();
			playbacksRegistrarMock.Invocations.Clear();

			// Act

			target.ReversePlaying();

			// Assert

			audioPlayerMock.Verify(x => x.Pause(), Times.Once);

			target.ReversePlayingKind.Should().Be("Play");

			playbacksRegistrarMock.Invocations.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ReversePlaying_ForPausedPlayer_ResumesPlaying()
		{
			// Arrange

			var audioPlayerMock = new Mock<IAudioPlayer>();
			var playbacksRegistrarMock = new Mock<ISongPlaybacksRegistrar>();

			var mocker = new AutoMocker();
			mocker.Use(audioPlayerMock);
			mocker.Use(playbacksRegistrarMock);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			await target.Play(new SongModel(), CancellationToken.None);
			target.ReversePlaying();
			audioPlayerMock.Invocations.Clear();
			playbacksRegistrarMock.Invocations.Clear();

			// Act

			target.ReversePlaying();

			// Assert

			audioPlayerMock.Verify(x => x.Play(), Times.Once);

			target.ReversePlayingKind.Should().Be("Pause");

			playbacksRegistrarMock.Invocations.Should().BeEmpty();
		}

		[TestMethod]
		public async Task StopPlaying_ForPlayingPlayer_StopsPlaying()
		{
			// Arrange

			var audioPlayerMock = new Mock<IAudioPlayer>();
			var playbacksRegistrarMock = new Mock<ISongPlaybacksRegistrar>();

			var mocker = new AutoMocker();
			mocker.Use(audioPlayerMock);
			mocker.Use(playbacksRegistrarMock);

			var target = mocker.CreateInstance<SongPlayerViewModel>();

			await target.Play(new SongModel(), CancellationToken.None);
			audioPlayerMock.Invocations.Clear();
			playbacksRegistrarMock.Invocations.Clear();

			// Act

			target.StopPlaying();

			// Assert

			audioPlayerMock.Verify(x => x.Stop(), Times.Once);

			target.ReversePlayingKind.Should().Be("Play");

			playbacksRegistrarMock.Invocations.Should().BeEmpty();
		}

		[TestMethod]
		public async Task SongMediaFinishedEventHandler_RegisterPlaybackFinish()
		{
			// Arrange

			var song = new SongModel();

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			await target.Play(song, CancellationToken.None);

			// Act

			mocker.SendMessage(new SongMediaFinishedEventArgs());

			// Assert

			var playbacksRegistrarMock = mocker.GetMock<ISongPlaybacksRegistrar>();
			playbacksRegistrarMock.Verify(x => x.RegisterPlaybackFinish(song, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SongMediaFinishedEventHandler_SetsReversePlayingKindToPause()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.StubMessenger();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			await target.Play(new SongModel(), CancellationToken.None);

			// Act

			mocker.SendMessage(new SongMediaFinishedEventArgs());

			// Assert

			target.ReversePlayingKind.Should().Be("Play");
		}

		[TestMethod]
		public async Task SongMediaFinishedEventHandler_SendsSongPlaybackFinishedEvent()
		{
			// Arrange

			var mocker = new AutoMocker();
			var messenger = mocker.StubMessenger();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			SongPlaybackFinishedEventArgs songPlaybackFinishedEvent = null;
			messenger.Register<SongPlaybackFinishedEventArgs>(this, (_, e) => e.RegisterEvent(ref songPlaybackFinishedEvent));

			await target.Play(new SongModel(), CancellationToken.None);

			// Act

			messenger.Send(new SongMediaFinishedEventArgs());

			// Assert

			songPlaybackFinishedEvent.Should().NotBeNull();
		}

		[TestMethod]
		public void AudioPlayerPropertyChangedEventHandler_ForSongLengthProperty_RaisesPropertyChangedEventForAffectedProperties()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mocker.GetMock<IAudioPlayer>()
				.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IAudioPlayer.SongLength)));

			// Assert

			var expectedProperties = new[]
			{
				nameof(SongPlayerViewModel.SongLength),
				nameof(SongPlayerViewModel.SongProgress),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void AudioPlayerPropertyChangedEventHandler_ForSongPositionProperty_RaisesPropertyChangedEventForAffectedProperties()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SongPlayerViewModel>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mocker.GetMock<IAudioPlayer>()
				.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IAudioPlayer.SongPosition)));

			// Assert

			var expectedProperties = new[]
			{
				nameof(SongPlayerViewModel.SongPosition),
				nameof(SongPlayerViewModel.SongProgress),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}
	}
}
