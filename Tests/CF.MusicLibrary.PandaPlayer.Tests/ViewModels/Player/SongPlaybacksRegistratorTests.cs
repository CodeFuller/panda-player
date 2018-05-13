using System;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Player;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels.Player
{
	[TestFixture]
	public class SongPlaybacksRegistratorTests
	{
		[Test]
		public void Constructor_WhenMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SongPlaybacksRegistrator(null, Substitute.For<IScrobbler>(), Substitute.For<IClock>()));
		}

		[Test]
		public void Constructor_WhenScrobblerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SongPlaybacksRegistrator(Substitute.For<IMusicLibrary>(), null, Substitute.For<IClock>()));
		}

		[Test]
		public void Constructor_WhenClockArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SongPlaybacksRegistrator(Substitute.For<IMusicLibrary>(), Substitute.For<IScrobbler>(), null));
		}

		[Test]
		public void RegisterPlaybackStart_CallsScrobblerUpdateNowPlayingCorrectly()
		{
			// Arrange

			var song = new Song
			{
				TrackNumber = 5,
				Title = "Some Title",
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Duration = TimeSpan.FromSeconds(12345),
			};

			Track scrobbledTrack = null;

			IScrobbler scrobblerMock = Substitute.For<IScrobbler>();
			scrobblerMock.UpdateNowPlaying(Arg.Do<Track>(arg => scrobbledTrack = arg));
			var target = new SongPlaybacksRegistrator(Substitute.For<IMusicLibrary>(), scrobblerMock, Substitute.For<IClock>());

			// Act

			target.RegisterPlaybackStart(song).Wait();

			// Assert

			scrobblerMock.Received(1).UpdateNowPlaying(Arg.Any<Track>());
			Assert.AreEqual(5, scrobbledTrack.Number);
			Assert.AreEqual("Some Title", scrobbledTrack.Title);
			Assert.AreEqual("Some Artist", scrobbledTrack.Artist);
			Assert.AreEqual("Some Artist", scrobbledTrack.Album.Artist);
			Assert.AreEqual("Some Album", scrobbledTrack.Album.Title);
			Assert.AreEqual(TimeSpan.FromSeconds(12345), scrobbledTrack.Duration);
		}

		[Test]
		public void RegisterPlaybackFinish_AddsSongPlaybackCorrectly()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();
			IClock clockStub = Substitute.For<IClock>();
			clockStub.Now.Returns(new DateTime(2017, 09, 23, 12, 36, 29));
			var target = new SongPlaybacksRegistrator(musicLibraryMock, Substitute.For<IScrobbler>(), clockStub);

			// Act

			target.RegisterPlaybackFinish(song).Wait();

			// Assert

			Assert.AreEqual(1, song.Playbacks.Count);
			var playback = song.Playbacks.Single();
			Assert.AreSame(song, playback.Song);
			Assert.AreEqual(new DateTime(2017, 09, 23, 12, 36, 29), playback.PlaybackTime);
			musicLibraryMock.Received(1).AddSongPlayback(song, new DateTime(2017, 09, 23, 12, 36, 29));
		}

		[Test]
		public void RegisterPlaybackFinish_ScrobblesTrackCorrectly()
		{
			// Arrange

			var song = new Song
			{
				TrackNumber = 5,
				Title = "Some Title",
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Duration = TimeSpan.FromMinutes(3),
			};

			TrackScrobble trackScrobble = null;

			IScrobbler scrobblerMock = Substitute.For<IScrobbler>();
			scrobblerMock.Scrobble(Arg.Do<TrackScrobble>(arg => trackScrobble = arg));
			IClock clockStub = Substitute.For<IClock>();
			clockStub.Now.Returns(new DateTime(2017, 09, 23, 12, 36, 29));
			var target = new SongPlaybacksRegistrator(Substitute.For<IMusicLibrary>(), scrobblerMock, clockStub);

			// Act

			target.RegisterPlaybackFinish(song).Wait();

			// Assert

			scrobblerMock.Received(1).Scrobble(Arg.Any<TrackScrobble>());
			var scrobbledTrack = trackScrobble.Track;
			Assert.IsTrue(trackScrobble.ChosenByUser);
			Assert.AreEqual(new DateTime(2017, 09, 23, 12, 33, 29), trackScrobble.PlayStartTimestamp);
			Assert.AreEqual(5, scrobbledTrack.Number);
			Assert.AreEqual("Some Title", scrobbledTrack.Title);
			Assert.AreEqual("Some Artist", scrobbledTrack.Artist);
			Assert.AreEqual("Some Artist", scrobbledTrack.Album.Artist);
			Assert.AreEqual("Some Album", scrobbledTrack.Album.Title);
			Assert.AreEqual(TimeSpan.FromMinutes(3), scrobbledTrack.Duration);
		}
	}
}
