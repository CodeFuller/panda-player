using System;
using CF.Library.Core.Logging;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LastFM.Objects;
using NSubstitute;
using NUnit.Framework;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.LastFM
{
	[TestFixture]
	public class LastFMScrobblerTests
	{
		[Test]
		public void Constructor_WhenLastFMApiClientArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new LastFMScrobbler(null));
		}

		[Test]
		public void UpdateNowPlaying_ForValidTrack_CallsLastFMApiCorrectly()
		{
			//	Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			};

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.UpdateNowPlaying(track).Wait();

			//	Assert

			lastFMApiClientMock.Received(1).UpdateNowPlaying(track);
		}

		[TestCase(null)]
		[TestCase("")]
		public void UpdateNowPlaying_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
		{
			//	Arrange

			var track = new Track
			{
				Title = title,
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			};

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.UpdateNowPlaying(track).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[TestCase(null)]
		[TestCase("")]
		public void UpdateNowPlaying_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
		{
			//	Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = artist,
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album(artist, "Some Album"),
			};

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.UpdateNowPlaying(track).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[Test]
		public void UpdateNowPlaying_WhenTrackIsTooShort_DoesNotCallLastFMApi()
		{
			//	Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromSeconds(15),
				Album = new Album("Some Artist", "Some Album"),
			};

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.UpdateNowPlaying(track).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[Test]
		public void Scrobble_ForValidTrack_CallsLastFMApiCorrectly()
		{
			//	Arrange

			var trackScrobble = new TrackScrobble(new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			}, new DateTime(2017, 09, 18));

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.Scrobble(trackScrobble).Wait();

			//	Assert

			lastFMApiClientMock.Received(1).Scrobble(trackScrobble);
		}

		[TestCase(null)]
		[TestCase("")]
		public void Scrobble_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
		{
			//	Arrange

			var trackScrobble = new TrackScrobble(new Track
			{
				Title = title,
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			}, new DateTime(2017, 09, 18));

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.Scrobble(trackScrobble).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}

		[TestCase(null)]
		[TestCase("")]
		public void Scrobble_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
		{
			//	Arrange

			var trackScrobble = new TrackScrobble(new Track
			{
				Title = "Some Title",
				Artist = artist,
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album(artist, "Some Album"),
			}, new DateTime(2017, 09, 18));

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.Scrobble(trackScrobble).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}

		[Test]
		public void Scrobble_WhenTrackIsTooShort_DoesNotCallLastFMApi()
		{
			//	Arrange

			var trackScrobble = new TrackScrobble(new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromSeconds(15),
				Album = new Album("Some Artist", "Some Album"),
			}, new DateTime(2017, 09, 18));

			Logger = Substitute.For<IMessageLogger>();
			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock);

			//	Act

			target.Scrobble(trackScrobble).Wait();

			//	Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}
	}
}
