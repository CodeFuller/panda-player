using System;
using Microsoft.Extensions.Logging;
using MusicLibrary.LastFM.Objects;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.LastFM.Tests
{
	[TestFixture]
	public class LastFMScrobblerTests
	{
		[Test]
		public void UpdateNowPlaying_ForValidTrack_CallsLastFMApiCorrectly()
		{
			// Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			lastFMApiClientMock.Received(1).UpdateNowPlaying(track);
		}

		[TestCase(null)]
		[TestCase("")]
		public void UpdateNowPlaying_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
		{
			// Arrange

			var track = new Track
			{
				Title = title,
				Artist = "Some Artist",
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album("Some Artist", "Some Album"),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[TestCase(null)]
		[TestCase("")]
		public void UpdateNowPlaying_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
		{
			// Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = artist,
				Duration = TimeSpan.FromMinutes(5),
				Album = new Album(artist, "Some Album"),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[Test]
		public void UpdateNowPlaying_WhenTrackIsTooShort_DoesNotCallLastFMApi()
		{
			// Arrange

			var track = new Track
			{
				Title = "Some Title",
				Artist = "Some Artist",
				Duration = TimeSpan.FromSeconds(15),
				Album = new Album("Some Artist", "Some Album"),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().UpdateNowPlaying(Arg.Any<Track>());
		}

		[Test]
		public void Scrobble_ForValidTrack_CallsLastFMApiCorrectly()
		{
			// Arrange

			var trackScrobble = new TrackScrobble
			{
				Track = new Track
				{
					Title = "Some Title",
					Artist = "Some Artist",
					Duration = TimeSpan.FromMinutes(5),
					Album = new Album("Some Artist", "Some Album"),
				},

				PlayStartTimestamp = new DateTime(2017, 09, 18),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			lastFMApiClientMock.Received(1).Scrobble(trackScrobble);
		}

		[TestCase(null)]
		[TestCase("")]
		public void Scrobble_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
		{
			// Arrange

			var trackScrobble = new TrackScrobble
			{
				Track = new Track
				{
					Title = title,
					Artist = "Some Artist",
					Duration = TimeSpan.FromMinutes(5),
					Album = new Album("Some Artist", "Some Album"),
				},

				PlayStartTimestamp = new DateTime(2017, 09, 18),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}

		[TestCase(null)]
		[TestCase("")]
		public void Scrobble_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
		{
			// Arrange

			var trackScrobble = new TrackScrobble
			{
				Track = new Track
				{
					Title = "Some Title",
					Artist = artist,
					Duration = TimeSpan.FromMinutes(5),
					Album = new Album(artist, "Some Album"),
				},

				PlayStartTimestamp = new DateTime(2017, 09, 18),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}

		[Test]
		public void Scrobble_WhenTrackIsTooShort_DoesNotCallLastFMApi()
		{
			// Arrange

			var trackScrobble = new TrackScrobble
			{
				Track = new Track
				{
					Title = "Some Title",
					Artist = "Some Artist",
					Duration = TimeSpan.FromSeconds(15),
					Album = new Album("Some Artist", "Some Album"),
				},

				PlayStartTimestamp = new DateTime(2017, 09, 18),
			};

			ILastFMApiClient lastFMApiClientMock = Substitute.For<ILastFMApiClient>();
			var target = new LastFMScrobbler(lastFMApiClientMock, Substitute.For<ILogger<LastFMScrobbler>>());

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			lastFMApiClientMock.DidNotReceive().Scrobble(Arg.Any<TrackScrobble>());
		}
	}
}
