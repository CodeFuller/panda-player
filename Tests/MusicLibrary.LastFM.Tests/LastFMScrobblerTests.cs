using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.LastFM.Objects;

namespace MusicLibrary.LastFM.Tests
{
	[TestClass]
	public class LastFMScrobblerTests
	{
		[TestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(track), Times.Once);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>()), Times.Never);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>()), Times.Never);
		}

		[TestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>()), Times.Never);
		}

		[TestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(trackScrobble), Times.Once);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>()), Times.Never);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>()), Times.Never);
		}

		[TestMethod]
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

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<LastFMScrobbler>();

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>()), Times.Never);
		}
	}
}
