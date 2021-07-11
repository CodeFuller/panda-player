using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.UnitTests.Extensions;
using MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.UnitTests.ViewModels.PersistentPlaylist
{
	[TestClass]
	public class PersistentSongPlaylistViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfNoPlaylistSessionDataExist_DoesNotLoadAnySongs()
		{
			// Arrange

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync((PlaylistData)null);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			NoPlaylistLoadedEventArgs noPlaylistLoadedEvent = null;
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref noPlaylistLoadedEvent));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			target.Songs.Should().BeEmpty();

			playlistLoadedEvent.Should().BeNull();
			noPlaylistLoadedEvent.Should().NotBeNull();
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfValidPlaylistSessionDataExist_LoadsPlaylistSongsCorrectly()
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
					new PlaylistSongData { Id = "song 2" },
					new PlaylistSongData { Id = "song 3" },
				},
				CurrentSongIndex = 1,
			};

			var songIds = new[]
			{
				new ItemId("song 0"),
				new ItemId("song 1"),
				new ItemId("song 2"),
				new ItemId("song 3"),
			};

			var songs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
				new SongModel { Id = new ItemId("song 2") },
				new SongModel { Id = new ItemId("song 3") },
			};

			// ISongsService.GetSongs() does not guarantee songs order.
			var songsReturnedByService = new[] { songs[3], songs[2], songs[0], songs[1] };

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(songIds, It.IsAny<CancellationToken>())).ReturnsAsync(songsReturnedByService);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			NoPlaylistLoadedEventArgs noPlaylistLoadedEvent = null;
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref noPlaylistLoadedEvent));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(songs[1]);

			playlistLoadedEvent.VerifyPlaylistEvent(songs, 1);
			noPlaylistLoadedEvent.Should().BeNull();

			propertyChangedEvents.VerifySongListPropertyChangedEvents();
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfLoadedPlaylistContainsDuplicatedSong_LoadsPlaylistCorrectly()
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
					new PlaylistSongData { Id = "song 0" },
				},
				CurrentSongIndex = 2,
			};

			var songs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
			};

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(It.IsAny<IEnumerable<ItemId>>(), It.IsAny<CancellationToken>())).ReturnsAsync(songs);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			var expectedSongs = new[]
			{
				songs[0],
				songs[1],
				songs[0],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(expectedSongs[2]);

			playlistLoadedEvent.VerifyPlaylistEvent(expectedSongs, 2);
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfCurrentSongInPlaylistDataIsNotSet_LoadsPlaylistCorrectly()
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
				},
				CurrentSongIndex = null,
			};

			var songs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
			};

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(It.IsAny<IEnumerable<ItemId>>(), It.IsAny<CancellationToken>())).ReturnsAsync(songs);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			target.Songs.Should().BeEquivalentTo(songs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().BeNull();

			playlistLoadedEvent.VerifyPlaylistEvent(songs, null);
		}

		[DataRow(-1)]
		[DataRow(3)]
		[DataTestMethod]
		public void ApplicationLoadedEventHandler_IfCurrentSongIsInvalid_DoesNotLoadPlaylist(int currentSongIndex)
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
					new PlaylistSongData { Id = "song 2" },
				},
				CurrentSongIndex = currentSongIndex,
			};

			var loadedSongs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
				new SongModel { Id = new ItemId("song 2") },
			};

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(It.IsAny<IEnumerable<ItemId>>(), It.IsAny<CancellationToken>())).ReturnsAsync(loadedSongs);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			NoPlaylistLoadedEventArgs noPlaylistLoadedEvent = null;
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref noPlaylistLoadedEvent));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			target.Songs.Should().BeEmpty();

			playlistLoadedEvent.Should().BeNull();
			noPlaylistLoadedEvent.Should().NotBeNull();
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfSomeSongsAreDeleted_SkipsDeletedSongsFromLoading()
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "active song 0" },
					new PlaylistSongData { Id = "deleted song 1" },
					new PlaylistSongData { Id = "active song 2" },
					new PlaylistSongData { Id = "deleted song 3" },
				},

				// Current song index is set between deleted songs, so that we could test index adjustment.
				CurrentSongIndex = 2,
			};

			var loadedSongs = new[]
			{
				new SongModel { Id = new ItemId("active song 2") },
				new SongModel
				{
					// Song title for deleted song is logged differently depending on whether it has track number or not.
					// That's why first deleted song has track number and 2nd does not.
					Id = new ItemId("deleted song 1"),
					TrackNumber = 1,
					DeleteDate = new DateTime(2021, 07, 11),
				},
				new SongModel { Id = new ItemId("active song 0") },
				new SongModel
				{
					Id = new ItemId("deleted song 3"),
					TrackNumber = null,
					DeleteDate = new DateTime(2021, 07, 11),
				},
			};

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(It.IsAny<IEnumerable<ItemId>>(), It.IsAny<CancellationToken>())).ReturnsAsync(loadedSongs);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			var expectedSongs = new[]
			{
				loadedSongs[2],
				loadedSongs[0],
			};

			target.Songs.Should().BeEquivalentTo(expectedSongs, x => x.WithStrictOrdering());
			target.CurrentSong.Should().Be(expectedSongs[1]);

			playlistLoadedEvent.VerifyPlaylistEvent(expectedSongs, 1);
		}

		[TestMethod]
		public void ApplicationLoadedEventHandler_IfSomeSongsAreNotFound_DoesNotLoadPlaylist()
		{
			// Arrange

			var playlistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
					new PlaylistSongData { Id = "song 2" },
				},
				CurrentSongIndex = 0,
			};

			var loadedSongs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 2") },
			};

			var sessionServiceStub = new Mock<ISessionDataService>();
			sessionServiceStub.Setup(x => x.GetData<PlaylistData>("SongPlaylistData", It.IsAny<CancellationToken>())).ReturnsAsync(playlistData);

			var songServiceStub = new Mock<ISongsService>();
			songServiceStub.Setup(x => x.GetSongs(It.IsAny<IEnumerable<ItemId>>(), It.IsAny<CancellationToken>())).ReturnsAsync(loadedSongs);

			var mocker = new AutoMocker();
			mocker.Use(sessionServiceStub);
			mocker.Use(songServiceStub);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			PlaylistLoadedEventArgs playlistLoadedEvent = null;
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref playlistLoadedEvent));

			NoPlaylistLoadedEventArgs noPlaylistLoadedEvent = null;
			Messenger.Default.Register<NoPlaylistLoadedEventArgs>(this, e => e.RegisterEvent(ref noPlaylistLoadedEvent));

			// Act

			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Assert

			target.Songs.Should().BeEmpty();

			playlistLoadedEvent.Should().BeNull();
			noPlaylistLoadedEvent.Should().NotBeNull();
		}

		[TestMethod]
		public void PlaylistFinishedEventHandler_PurgesPlaylistData()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			// Act

			Messenger.Default.Send(new PlaylistFinishedEventArgs(Enumerable.Empty<SongModel>()));

			// Assert

			mocker.GetMock<ISessionDataService>().Verify(x => x.PurgeData("SongPlaylistData", It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SwitchToNextSong_SavesCorrectPlaylistData()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
				new SongModel { Id = new ItemId("song 2") },
			};

			PlaylistData savedPlaylistData = null;
			var sessionDataServiceMock = new Mock<ISessionDataService>();
			sessionDataServiceMock.Setup(x => x.SaveData("SongPlaylistData", It.IsAny<PlaylistData>(), It.IsAny<CancellationToken>()))
				.Callback<string, PlaylistData, CancellationToken>((_, data, _) => savedPlaylistData = data);

			var mocker = new AutoMocker();
			mocker.Use(sessionDataServiceMock);

			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			target.SetSongs(songs);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			savedPlaylistData = null;
			sessionDataServiceMock.Invocations.Clear();

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			var expectedPlaylistData = new PlaylistData
			{
				Songs = new[]
				{
					new PlaylistSongData { Id = "song 0" },
					new PlaylistSongData { Id = "song 1" },
					new PlaylistSongData { Id = "song 2" },
				},
				CurrentSongIndex = 2,
			};

			sessionDataServiceMock.Verify(x => x.SaveData(It.IsAny<string>(), It.IsAny<PlaylistData>(), It.IsAny<CancellationToken>()), Times.Once);
			savedPlaylistData.Should().BeEquivalentTo(expectedPlaylistData);
		}

		// With this test we actually check that PersistentSongPlaylistViewModel.OnPlaylistChanged() invokes base.OnPlaylistChanged()
		[TestMethod]
		public async Task SwitchToNextSong_SendsPlaylistChangedEvent()
		{
			// Arrange

			var songs = new[]
			{
				new SongModel { Id = new ItemId("song 0") },
				new SongModel { Id = new ItemId("song 1") },
				new SongModel { Id = new ItemId("song 2") },
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PersistentSongPlaylistViewModel>();

			target.SetSongs(songs);
			await target.SwitchToNextSong(CancellationToken.None);
			await target.SwitchToNextSong(CancellationToken.None);

			PlaylistChangedEventArgs playlistChangedEventArgs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => e.RegisterEvent(ref playlistChangedEventArgs));

			// Act

			await target.SwitchToNextSong(CancellationToken.None);

			// Assert

			playlistChangedEventArgs.VerifyPlaylistEvent(songs, 2);
		}
	}
}
