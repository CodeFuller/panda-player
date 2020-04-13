using System;
using Microsoft.Extensions.Logging;
using MusicLibrary.LastFM;
using MusicLibrary.LastFM.Objects;
using MusicLibrary.PandaPlayer.ViewModels.Scrobbling;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests.ViewModels.Scrobbling
{
	[TestFixture]
	public class PersistentScrobblerTests
	{
		[Test]
		public void UpdateNowPlaying_InvokesScrobblerCorrectly()
		{
			// Arrange

			var track = new Track();

			var scrobblerMock = Substitute.For<IScrobbler>();

			var target = new PersistentScrobbler(scrobblerMock, Substitute.For<IScrobblesProcessor>(), Substitute.For<ILogger<PersistentScrobbler>>());

			// Act

			target.UpdateNowPlaying(track).Wait();

			// Assert

			scrobblerMock.Received(1).UpdateNowPlaying(track);
		}

		[Test]
		public void UpdateNowPlaying_IfScrobblerThrows_DoesNotThrow()
		{
			// Arrange

			var scrobblerStub = Substitute.For<IScrobbler>();
			scrobblerStub.UpdateNowPlaying(Arg.Any<Track>()).Throws(new InvalidOperationException());

			var target = new PersistentScrobbler(scrobblerStub, Substitute.For<IScrobblesProcessor>(), Substitute.For<ILogger<PersistentScrobbler>>());

			// Act & Assert

			Assert.DoesNotThrow(() => target.UpdateNowPlaying(new Track()).Wait());
		}

		[Test]
		public void Scrobble_InvokesScrobblesProcessorCorrectly()
		{
			// Arrange

			var trackScrobble = new TrackScrobble();

			var scrobbler = Substitute.For<IScrobbler>();
			var scrobblesProcessorMock = Substitute.For<IScrobblesProcessor>();

			var target = new PersistentScrobbler(scrobbler, scrobblesProcessorMock, Substitute.For<ILogger<PersistentScrobbler>>());

			// Act

			target.Scrobble(trackScrobble).Wait();

			// Assert

			scrobblesProcessorMock.Received(1).ProcessScrobble(trackScrobble, scrobbler);
		}
	}
}
