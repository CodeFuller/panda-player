using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.Library.Core.Facades;
using CF.MusicLibrary.LastFM.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Scrobbling;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels.Scrobbling
{
	[TestFixture]
	public class ScrobblesQueueRepositoryTests
	{
		[Test]
		public void Load_ForCorrectlyStoredScrobble_LoadsScrobbleCorrectly()
		{
			// Arrange

			var track = new Track
			{
				Number = 5,
				Title = "Some Title",
				Artist = "Some Artist",
				Album = new Album("Some Artist", "Some Album"),
				Duration = new TimeSpan(1, 2, 3),
			};

			var trackScrobble = new TrackScrobble
			{
				Track = track,
				PlayStartTimestamp = new DateTime(2018, 05, 12, 21, 22, 19),
				ChosenByUser = true,
			};

			var target = new ScrobblesQueueRepository(GetFileSystemStub("SomeFile.txt"), Substitute.For<ILogger<ScrobblesQueueRepository>>(), "SomeFile.txt");

			// Act

			target.Save(new Queue<TrackScrobble>(new[] { trackScrobble }));
			var loadedScrobbles = target.Load();

			// Assert

			var loadedTrackScrobble = loadedScrobbles?.FirstOrDefault();
			Assert.IsNotNull(loadedTrackScrobble);
			var loadedTrack = loadedTrackScrobble.Track;
			Assert.IsNotNull(loadedTrack);
			Assert.AreEqual(track.Number, loadedTrack.Number);
			Assert.AreEqual(track.Title, loadedTrack.Title);
			Assert.AreEqual(track.Artist, loadedTrack.Artist);
			Assert.AreEqual(track.Album.Artist, loadedTrack.Album.Artist);
			Assert.AreEqual(track.Album.Title, loadedTrack.Album.Title);
			Assert.AreEqual(track.Duration, loadedTrack.Duration);
			Assert.AreEqual(trackScrobble.PlayStartTimestamp, loadedTrackScrobble.PlayStartTimestamp);
			Assert.AreEqual(trackScrobble.ChosenByUser, loadedTrackScrobble.ChosenByUser);
		}

		private IFileSystemFacade GetFileSystemStub(string fileName)
		{
			string writtenData = null;
			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(fileName).Returns(true);
			fileSystemStub.WriteAllText(fileName, Arg.Do<string>(arg => writtenData = arg), Encoding.UTF8);
			fileSystemStub.ReadAllText(fileName, Encoding.UTF8).Returns(x => writtenData);

			return fileSystemStub;
		}
	}
}
