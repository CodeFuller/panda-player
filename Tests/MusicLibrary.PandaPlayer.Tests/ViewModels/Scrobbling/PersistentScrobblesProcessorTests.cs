using System;
using System.Collections.Generic;
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
	public class PersistentScrobblesProcessorTests
	{
		[Test]
		public void ProcessScrobble_OnFirstCall_LoadsQueue()
		{
			// Arrange

			var prevScrobble1 = new TrackScrobble();
			var prevScrobble2 = new TrackScrobble();
			var newScrobble = new TrackScrobble();

			var queueMock = Substitute.For<Queue<TrackScrobble>>();
			var repositoryStub = Substitute.For<IScrobblesQueueRepository>();
			repositoryStub.Load().Returns(new Queue<TrackScrobble>(new[] { prevScrobble1, prevScrobble2 }));

			var target = new PersistentScrobblesProcessor(queueMock, repositoryStub, Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(newScrobble, Substitute.For<IScrobbler>()).Wait();

			// Assert

			Received.InOrder(() =>
			{
				queueMock.Received(1).Enqueue(prevScrobble1);
				queueMock.Received(1).Enqueue(prevScrobble2);
				queueMock.Received(1).Enqueue(newScrobble);
			});
		}

		[Test]
		public void ProcessScrobble_OnFirstCallIfRepositoryDoesNotProvideData_InitializesEmptyQueue()
		{
			// Arrange

			var newScrobble = new TrackScrobble();

			var queueMock = Substitute.For<Queue<TrackScrobble>>();
			var repositoryStub = Substitute.For<IScrobblesQueueRepository>();
			repositoryStub.Load().Returns((Queue<TrackScrobble>)null);

			var target = new PersistentScrobblesProcessor(queueMock, repositoryStub, Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(newScrobble, Substitute.For<IScrobbler>()).Wait();

			// Assert

			queueMock.Received(1).Enqueue(Arg.Any<TrackScrobble>());
			queueMock.Received(1).Enqueue(newScrobble);
		}

		[Test]
		public void ProcessScrobble_OnSubsequentCalls_DoesNotLoadQueue()
		{
			// Arrange

			var repositoryMock = Substitute.For<IScrobblesQueueRepository>();
			var target = new PersistentScrobblesProcessor(new Queue<TrackScrobble>(), repositoryMock, Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			target.ProcessScrobble(new TrackScrobble(), Substitute.For<IScrobbler>()).Wait();
			repositoryMock.ClearReceivedCalls();

			// Act

			target.ProcessScrobble(new TrackScrobble(), Substitute.For<IScrobbler>()).Wait();

			// Assert

			repositoryMock.DidNotReceive().Load();
		}

		[Test]
		public void ProcessScrobble_IfQueueIsEmpty_ProcessesNewScrobble()
		{
			// Arrange

			var scrobble = new TrackScrobble();
			var scrobblerMock = Substitute.For<IScrobbler>();
			var target = new PersistentScrobblesProcessor(new Queue<TrackScrobble>(), Substitute.For<IScrobblesQueueRepository>(), Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(scrobble, scrobblerMock).Wait();

			// Assert

			scrobblerMock.Received(1).Scrobble(scrobble);
		}

		[Test]
		public void ProcessScrobble_IfQueueIsNotEmpty_ProcessesScrobblesInCorrectOrder()
		{
			// Arrange

			var prevScrobble1 = new TrackScrobble();
			var prevScrobble2 = new TrackScrobble();
			var newScrobble = new TrackScrobble();

			var scrobblerMock = Substitute.For<IScrobbler>();
			var target = new PersistentScrobblesProcessor(
				new Queue<TrackScrobble>(new[] { prevScrobble1, prevScrobble2 }),
				Substitute.For<IScrobblesQueueRepository>(), Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(newScrobble, scrobblerMock).Wait();

			// Assert

			Received.InOrder(() =>
			{
				scrobblerMock.Received(1).Scrobble(prevScrobble1);
				scrobblerMock.Received(1).Scrobble(prevScrobble2);
				scrobblerMock.Received(1).Scrobble(newScrobble);
			});
		}

		[Test]
		public void ProcessScrobble_IfScrobblerThrows_DoesNotThrow()
		{
			// Arrange

			var scrobblerStub = Substitute.For<IScrobbler>();
			scrobblerStub.Scrobble(Arg.Any<TrackScrobble>()).Throws<InvalidOperationException>();

			var target = new PersistentScrobblesProcessor(new Queue<TrackScrobble>(), Substitute.For<IScrobblesQueueRepository>(), Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act & Assert

			Assert.DoesNotThrow(() => target.ProcessScrobble(new TrackScrobble(), scrobblerStub).Wait());
		}

		[Test]
		public void ProcessScrobble_IfAllScrobblesAreProcessed_PurgesRepositoryData()
		{
			// Arrange

			var repositoryMock = Substitute.For<IScrobblesQueueRepository>();

			var target = new PersistentScrobblesProcessor(new Queue<TrackScrobble>(new[] { new TrackScrobble() }), repositoryMock, Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(new TrackScrobble(), Substitute.For<IScrobbler>()).Wait();

			// Assert

			repositoryMock.Received(1).Purge();
			repositoryMock.DidNotReceive().Save(Arg.Any<Queue<TrackScrobble>>());
		}

		[Test]
		public void ProcessScrobble_IfSomeScrobblesLeftUnprocessed_SavesThoseScrobblesInRepository()
		{
			// Arrange

			var processedScrobble = new TrackScrobble();
			var failedScrobble = new TrackScrobble();
			var newScrobble = new TrackScrobble();

			var scrobblerStub = Substitute.For<IScrobbler>();
			scrobblerStub.Scrobble(failedScrobble).Throws<InvalidOperationException>();

			Queue<TrackScrobble> savedData = null;

			var queue = new Queue<TrackScrobble>(new[] { processedScrobble, failedScrobble });
			var repositoryMock = Substitute.For<IScrobblesQueueRepository>();
			repositoryMock.Save(Arg.Do<Queue<TrackScrobble>>(data => savedData = new Queue<TrackScrobble>(data)));

			var target = new PersistentScrobblesProcessor(queue, repositoryMock, Substitute.For<ILogger<PersistentScrobblesProcessor>>());

			// Act

			target.ProcessScrobble(newScrobble, scrobblerStub).Wait();

			// Assert

			CollectionAssert.AreEqual(new[] { failedScrobble, newScrobble }, savedData);
			CollectionAssert.AreEqual(new[] { failedScrobble, newScrobble }, queue);
		}
	}
}
