using System;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.Library.Core.Logging;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	[TestFixture]
	public class PersistentSongPlaylistViewModelTests
	{
		private class PersistentSongPlaylistViewModelInheritor : PersistentSongPlaylistViewModel
		{
			public PersistentSongPlaylistViewModelInheritor(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IGenericDataRepository<PlaylistData> playlistDataRepository)
				: base(libraryContentUpdater, viewNavigator, playlistDataRepository)
			{
			}

			public void InvokeOnPlaylistChanged()
			{
				base.OnPlaylistChanged();
			}
		}

		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Constructor_IfLibraryContentUpdaterArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new PersistentSongPlaylistViewModel(null,
				Substitute.For<IViewNavigator>(), Substitute.For<IGenericDataRepository<PlaylistData>>()));
		}

		[Test]
		public void Constructor_IfViewNavigatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(),
				null, Substitute.For<IGenericDataRepository<PlaylistData>>()));
		}

		[Test]
		public void Constructor_IfPlaylistDataRepositoryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(),
				Substitute.For<IViewNavigator>(), null));
		}

		[Test]
		public void LibraryLoadedEventHandler_IfPlaylistDataRepositoryReturnsNoData_ReturnsWithNoAction()
		{
			//	Arrange

			var song = new Song();
			var disc = new Disc { SongsUnordered = new[] { song } };

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns((PlaylistData)null);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			Assert.IsEmpty(target.Songs);
		}

		[Test]
		public void LibraryLoadedEventHandler_IfPlaylistDataIsCorrect_LoadsPlaylistSongsCorrectly()
		{
			//	Arrange

			var song1 = new Song { Id = 1 };
			var song2 = new Song { Id = 2 };
			var disc = new Disc { SongsUnordered = new[] { song1, song2 } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song1), new PlaylistSongData(song2) }.ToCollection(),
				CurrentSongIndex = 1,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns(playlistData);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			CollectionAssert.AreEqual(new[] { song1, song2 }, target.Songs);
			Assert.AreSame(song2, target.CurrentSong);
		}

		[Test]
		public void LibraryLoadedEventHandler_IfCurrentSongIsNotSet_LoadsPlaylistCorrectly()
		{
			//	Arrange

			var song = new Song { Id = 1 };
			var disc = new Disc { SongsUnordered = new[] { song } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song) }.ToCollection(),
				CurrentSongIndex = null,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns(playlistData);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			CollectionAssert.AreEqual(new[] { song }, target.Songs);
			Assert.IsNull(target.CurrentSong);
		}

		[Test]
		public void LibraryLoadedEventHandler_IfPlaylistDataContainsSomeUnexistingSongs_ReturnsWithNoAction()
		{
			//	Arrange

			var song = new Song { Id = 1 };
			var disc = new Disc { SongsUnordered = new[] { song } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song), new PlaylistSongData(new Song { Id = 2 }) }.ToCollection(),
				CurrentSongIndex = 0,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns(playlistData);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			Assert.IsEmpty(target.Songs);
		}

		[TestCase(-1)]
		[TestCase(1)]
		public void LibraryLoadedEventHandler_IfCurrentSongIndexIsInvalid_ReturnsWithNoAction(int savedSongIndex)
		{
			//	Arrange

			var song = new Song { Id = 1 };
			var disc = new Disc { SongsUnordered = new[] { song } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song) }.ToCollection(),
				CurrentSongIndex = savedSongIndex,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns(playlistData);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			Assert.IsEmpty(target.Songs);
		}

		[Test]
		public void LibraryLoadedEventHandler_IfPlaylistDataWasLoaded_DoesNotSaveUpdatedPlaylist()
		{
			//	Arrange

			var song = new Song { Id = 1 };
			var disc = new Disc { SongsUnordered = new[] { song } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song) }.ToCollection(),
				CurrentSongIndex = 0,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryMock = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryMock.Load().Returns(playlistData);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryMock);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			//	Paranoick check
			Assert.IsNotEmpty(target.Songs);
			playlistDataRepositoryMock.DidNotReceive().Save(Arg.Any<PlaylistData>());
		}

		[Test]
		public void LibraryLoadedEventHandler_IfPlaylistDataWasLoaded_SendsPlaylistLoadedEvent()
		{
			//	Arrange

			var song = new Song { Id = 1 };
			var disc = new Disc { SongsUnordered = new[] { song } };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song) }.ToCollection(),
				CurrentSongIndex = 0,
			};

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryStub = Substitute.For<IGenericDataRepository<PlaylistData>>();
			playlistDataRepositoryStub.Load().Returns(playlistData);

			bool receivedEvent = false;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => receivedEvent = true);

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryStub);

			//	Act

			Messenger.Default.Send(new LibraryLoadedEventArgs(new DiscLibrary(Enumerable.Repeat(disc, 1))));

			//	Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void OnPlaylistChanged_SavesPlaylistDataCorrectly()
		{
			//	Arrange

			var song1 = new Song { Id = 1, Uri = new Uri("/SongUri1", UriKind.Relative) };
			var song2 = new Song { Id = 2, Uri = new Uri("/SongUri2", UriKind.Relative) };
			var songs = new[] { song1, song2 };

			PlaylistData savedPlaylistData = null;

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryMock = Substitute.For<IGenericDataRepository<PlaylistData>>();

			var target = new PersistentSongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryMock);
			target.SetSongs(songs);
			target.SwitchToSong(song2);
			playlistDataRepositoryMock.ClearReceivedCalls();
			playlistDataRepositoryMock.Save(Arg.Do<PlaylistData>(arg => savedPlaylistData = arg));

			//	Act

			target.InvokeOnPlaylistChanged();

			//	Assert

			playlistDataRepositoryMock.Received(1).Save(Arg.Any<PlaylistData>());
			var savedSongs = savedPlaylistData.Songs.ToList();
			Assert.AreEqual(2, savedSongs.Count);
			Assert.AreEqual(1, savedSongs[0].Id);
			Assert.AreEqual(new Uri("/SongUri1", UriKind.Relative), savedSongs[0].Uri);
			Assert.AreEqual(2, savedSongs[1].Id);
			Assert.AreEqual(new Uri("/SongUri2", UriKind.Relative), savedSongs[1].Uri);
			Assert.AreEqual(1, savedPlaylistData.CurrentSongIndex);
		}

		[Test]
		public void OnPlaylistChanged_WhenPlaylistCurrentSongIsNotSet_SavesPlaylistDataCorrectly()
		{
			//	Arrange

			var songs = new[] { new Song() };

			PlaylistData savedPlaylistData = null;

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryMock = Substitute.For<IGenericDataRepository<PlaylistData>>();

			var target = new PersistentSongPlaylistViewModelInheritor(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryMock);
			target.SetSongs(songs);
			playlistDataRepositoryMock.ClearReceivedCalls();
			playlistDataRepositoryMock.Save(Arg.Do<PlaylistData>(arg => savedPlaylistData = arg));

			//	Act

			target.InvokeOnPlaylistChanged();

			//	Assert

			playlistDataRepositoryMock.Received(1).Save(Arg.Any<PlaylistData>());
			Assert.IsNull(savedPlaylistData.CurrentSongIndex);
		}

		[Test]
		public void PlaylistFinishedEventHandler_PurgesPlaylistData()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IGenericDataRepository<PlaylistData> playlistDataRepositoryMock = Substitute.For<IGenericDataRepository<PlaylistData>>();

			var target = new PersistentSongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), playlistDataRepositoryMock);

			//	Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(target));

			//	Assert

			playlistDataRepositoryMock.Received(1).Purge();
		}
	}
}
