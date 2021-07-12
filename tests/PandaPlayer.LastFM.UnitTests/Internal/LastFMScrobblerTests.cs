using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.LastFM.Interfaces;
using PandaPlayer.LastFM.Internal;
using PandaPlayer.LastFM.Objects;

namespace PandaPlayer.LastFM.UnitTests.Internal
{
	[TestClass]
	public class LastFMScrobblerTests
	{
		[TestMethod]
		public async Task UpdateNowPlaying_ForValidTrack_CallsLastFMApiCorrectly()
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

			await target.UpdateNowPlaying(track, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(track, It.IsAny<CancellationToken>()), Times.Once);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
		public async Task UpdateNowPlaying_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
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

			await target.UpdateNowPlaying(track, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
		public async Task UpdateNowPlaying_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
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

			await target.UpdateNowPlaying(track, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task UpdateNowPlaying_WhenTrackIsTooShort_DoesNotCallLastFMApi()
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

			await target.UpdateNowPlaying(track, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.UpdateNowPlaying(It.IsAny<Track>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task Scrobble_ForValidTrack_CallsLastFMApiCorrectly()
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

			await target.Scrobble(trackScrobble, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(trackScrobble, It.IsAny<CancellationToken>()), Times.Once);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
		public async Task Scrobble_WhenTrackDoesNotHaveTitle_DoesNotCallLastFMApi(string title)
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

			await target.Scrobble(trackScrobble, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[DataRow(null)]
		[DataRow("")]
		[DataTestMethod]
		public async Task Scrobble_WhenTrackDoesNotHaveArtist_DoesNotCallLastFMApi(string artist)
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

			await target.Scrobble(trackScrobble, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task Scrobble_WhenTrackIsTooShort_DoesNotCallLastFMApi()
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

			await target.Scrobble(trackScrobble, CancellationToken.None);

			// Assert

			var apiClientMock = mocker.GetMock<ILastFMApiClient>();
			apiClientMock.Verify(x => x.Scrobble(It.IsAny<TrackScrobble>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
