using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Facades;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.UnitTests.ViewModels.Player
{
	[TestClass]
	public class AudioPlayerTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void SongPositionGetter_ReturnsCurrentMediaPosition()
		{
			// Arrange

			var mediaPlayerStub = new Mock<IMediaPlayerFacade>();
			mediaPlayerStub.Setup(x => x.Position).Returns(TimeSpan.FromSeconds(42));

			var mocker = new AutoMocker();
			mocker.Use(mediaPlayerStub);

			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			var songPosition = target.SongPosition;

			// Assert

			songPosition.Should().Be(TimeSpan.FromSeconds(42));
		}

		[TestMethod]
		public void SongPositionSetter_SetsMediaPlayerPosition()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.SongPosition = TimeSpan.FromSeconds(42);

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.VerifySet(x => x.Position = TimeSpan.FromSeconds(42), Times.Once);

			var expectedProperties = new[]
			{
				nameof(AudioPlayer.SongPosition),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void VolumeGetter_ReturnsMediaPlayerVolume()
		{
			// Arrange

			var mediaPlayerStub = new Mock<IMediaPlayerFacade>();
			mediaPlayerStub.Setup(x => x.Volume).Returns(0.123);

			var mocker = new AutoMocker();
			mocker.Use(mediaPlayerStub);

			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			var volume = target.Volume;

			// Assert

			volume.Should().Be(0.123);
		}

		[TestMethod]
		public void VolumeSetter_SetsMediaPlayerVolume()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			target.Volume = 0.123;

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.VerifySet(x => x.Volume = 0.123, Times.Once);
		}

		[TestMethod]
		public void Constructor_SetsTimerInterval()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Act

			var target = mocker.CreateInstance<AudioPlayer>();

			// Assert

			var timerMock = mocker.GetMock<ITimerFacade>();
			timerMock.VerifySet(x => x.Interval = 200, Times.Once);
		}

		[TestMethod]
		public void MediaOpenedEventHandler_SetsSongLength()
		{
			// Arrange

			var mediaPlayerStub = new Mock<IMediaPlayerFacade>();
			mediaPlayerStub.Setup(x => x.NaturalDuration).Returns(new Duration(TimeSpan.FromSeconds(123)));

			var mocker = new AutoMocker();
			mocker.Use(mediaPlayerStub);

			var target = mocker.CreateInstance<AudioPlayer>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mediaPlayerStub.Raise(x => x.MediaOpened += null, new EventArgs());

			// Assert

			target.SongLength.Should().Be(TimeSpan.FromSeconds(123));

			var expectedProperties = new[]
			{
				nameof(AudioPlayer.SongLength),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void MediaEndedEventHandler_StopsTimer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			mocker.GetMock<IMediaPlayerFacade>().Raise(x => x.MediaEnded += null, new EventArgs());

			// Assert

			var timerMock = mocker.GetMock<ITimerFacade>();
			timerMock.Verify(x => x.Stop(), Times.Once);
		}

		[TestMethod]
		public void MediaEndedEventHandler_ClosesMediaPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			mocker.GetMock<IMediaPlayerFacade>().Raise(x => x.MediaEnded += null, new EventArgs());

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.Verify(x => x.Close(), Times.Once);
		}

		[TestMethod]
		public void MediaEndedEventHandler_SendsSongMediaFinishedEvent()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			SongMediaFinishedEventArgs songMediaFinishedEvent = null;
			Messenger.Default.Register<SongMediaFinishedEventArgs>(this, e => e.RegisterEvent(ref songMediaFinishedEvent));

			// Act

			mocker.GetMock<IMediaPlayerFacade>().Raise(x => x.MediaEnded += null, new EventArgs());

			// Assert

			songMediaFinishedEvent.Should().NotBeNull();
		}

		[TestMethod]
		public void MediaEndedEventHandler_SetsSongPositionAndLengthToZero()
		{
			// Arrange

			var playerPosition = TimeSpan.FromSeconds(42);

			var mediaPlayerMock = new Mock<IMediaPlayerFacade>();
			mediaPlayerMock.Setup(x => x.NaturalDuration).Returns(new Duration(TimeSpan.FromSeconds(123)));
			mediaPlayerMock.Setup(x => x.Position).Returns(() => playerPosition);
			mediaPlayerMock.Setup(x => x.Close()).Callback(() => playerPosition = TimeSpan.Zero);

			var mocker = new AutoMocker();
			mocker.Use(mediaPlayerMock);

			var target = mocker.CreateInstance<AudioPlayer>();

			// Triggering MediaOpened event and calling Play() so that SongPosition and SongLength are set.
			mediaPlayerMock.Raise(x => x.MediaOpened += null, new EventArgs());
			target.Play();
			target.SongPosition.Should().NotBe(TimeSpan.Zero);
			target.SongLength.Should().NotBe(TimeSpan.Zero);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mediaPlayerMock.Raise(x => x.MediaEnded += null, new EventArgs());

			// Assert

			target.SongPosition.Should().Be(TimeSpan.Zero);
			target.SongLength.Should().Be(TimeSpan.Zero);

			mediaPlayerMock.VerifySet(x => x.Position = It.IsAny<TimeSpan>(), Times.Never);

			var expectedProperties = new[]
			{
				nameof(AudioPlayer.SongPosition),
				nameof(AudioPlayer.SongLength),
			};

			// We check for strict ordering of PropertyChangedEvent here, i.e. that SongPosition is updated before SongLength.
			// Otherwise, position / length is displayed oddly when track is switched - 4:12 / 0:00
			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public void TimerElapsedEventHandler_RaisesPropertyChangedEventForSongPosition()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			mocker.GetMock<ITimerFacade>().Raise(x => x.Elapsed += null, CreateElapsedEvent());

			// Assert

			var expectedProperties = new[]
			{
				nameof(AudioPlayer.SongPosition),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public void Open_OpensMediaPlayer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			target.Open(new Uri("http://wwww.example.com/"));

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.Verify(x => x.Open(new Uri("http://wwww.example.com/")), Times.Once);
		}

		[TestMethod]
		public void Play_PlaysMedia()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			target.Play();

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.Verify(x => x.Play(), Times.Once);
		}

		[TestMethod]
		public void Play_StartsTimer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			// Act

			target.Play();

			// Assert

			var timerMock = mocker.GetMock<ITimerFacade>();
			timerMock.Verify(x => x.Start(), Times.Once);
		}

		// See comment for State property in AudioPlayer.
		[TestMethod]
		public void Play_ForPausedPlayer_DoesNotSendPropertyChangedEventForSongPosition()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			target.Pause();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.Play();

			// Assert

			propertyChangedEvents.Should().BeEmpty();
		}

		[TestMethod]
		public void Pause_PausesMedia()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			target.Play();

			// Act

			target.Pause();

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.Verify(x => x.Pause(), Times.Once);
		}

		[TestMethod]
		public void Pause_StopsTimer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			target.Play();

			// Act

			target.Pause();

			// Assert

			var timerMock = mocker.GetMock<ITimerFacade>();
			timerMock.Verify(x => x.Stop(), Times.Once);
		}

		[TestMethod]
		public void Stop_StopsMedia()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			target.Play();

			// Act

			target.Stop();

			// Assert

			var mediaPlayerMock = mocker.GetMock<IMediaPlayerFacade>();
			mediaPlayerMock.Verify(x => x.Stop(), Times.Once);
		}

		[TestMethod]
		public void Stop_StopsTimer()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<AudioPlayer>();

			target.Play();

			// Act

			target.Stop();

			// Assert

			var timerMock = mocker.GetMock<ITimerFacade>();
			timerMock.Verify(x => x.Stop(), Times.Once);
		}

		// We test raising of PropertyChangedEvent on SongPosition property only for Stop() method and not for Play() or Pause().
		// For Play() SongPosition is updated by timer event.
		// For Pause() no SongPosition update is necessary.
		// For Stop() SongPosition is reset to zero.
		[TestMethod]
		public void Stop_UpdatesSongPositionCorrectly()
		{
			// Arrange

			var playerPosition = TimeSpan.FromSeconds(42);

			var mediaPlayerStub = new Mock<IMediaPlayerFacade>();
			mediaPlayerStub.Setup(x => x.Position).Returns(() => playerPosition);
			mediaPlayerStub.Setup(x => x.Stop()).Callback(() => playerPosition = TimeSpan.Zero);

			var mocker = new AutoMocker();
			mocker.Use(mediaPlayerStub);

			var target = mocker.CreateInstance<AudioPlayer>();

			target.Play();

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			target.Stop();

			// Assert

			target.SongPosition.Should().Be(TimeSpan.Zero);

			var expectedProperties = new[]
			{
				nameof(AudioPlayer.SongPosition),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Should().BeEquivalentTo(expectedProperties);
		}

		// https://github.com/dotnet/runtime/issues/31204
		private static ElapsedEventArgs CreateElapsedEvent()
		{
			var constructor = typeof(ElapsedEventArgs)
				.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
				.Single();

			return (ElapsedEventArgs)constructor.Invoke(new object[] { new DateTime(123) });
		}
	}
}
