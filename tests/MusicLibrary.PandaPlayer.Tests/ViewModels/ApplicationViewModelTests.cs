using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests.ViewModels
{
	[TestFixture]
	public class ApplicationViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void Load_LoadsDiscLibrary()
		{
			// Arrange

			bool libraryLoaded = false;
			var library = new DiscLibrary(() =>
			{
				libraryLoaded = true;
				return Task.FromResult(Enumerable.Empty<Disc>());
			});
			var target = new ApplicationViewModel(library, Substitute.For<IApplicationViewModelHolder>(), Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());

			// Act

			target.Load().Wait();

			// Assert

			Assert.IsTrue(libraryLoaded);
		}

		[Test]
		public void Load_AfterLibraryIsLoaded_SendsLibraryLoadedEvent()
		{
			// Arrange

			bool receivedEvent = false;

			var library = new DiscLibrary(() =>
			{
				// Event should be sent after library loading.
				receivedEvent = false;
				return Task.FromResult(Enumerable.Empty<Disc>());
			});

			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => receivedEvent = true);
			var target = new ApplicationViewModel(library, Substitute.For<IApplicationViewModelHolder>(), Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());

			// Act

			target.Load().Wait();

			// Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void Load_IfPreviousPlaylistDataWasLoaded_SwitchesToPlaylistView()
		{
			// Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.Songs.Returns(new[] { new Song() });
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(), musicPlayerViewModelStub, Substitute.For<IViewNavigator>());

			// Act

			target.Load().Wait();

			// Assert

			Assert.AreEqual(1, target.SelectedSongListIndex);
		}

		[Test]
		public void Load_IfPreviousPlaylistDataWasNotLoaded_SwitchesToExplorerSongListView()
		{
			// Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.Songs.Returns(Enumerable.Empty<Song>());
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(), musicPlayerViewModelStub, Substitute.For<IViewNavigator>());

			// Act

			target.Load().Wait();

			// Assert

			Assert.AreEqual(0, target.SelectedSongListIndex);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenExplorerSongListSelected_SendsActiveDiscChangedEventForCurrentExplorerDisc()
		{
			// Arrange

			var explorerDisc = new Disc();

			ILibraryExplorerViewModel libraryExplorerViewModelStub = Substitute.For<ILibraryExplorerViewModel>();
			libraryExplorerViewModelStub.SelectedDisc.Returns(explorerDisc);
			IApplicationViewModelHolder viewModelHolderStub = Substitute.For<IApplicationViewModelHolder>();
			viewModelHolderStub.LibraryExplorerViewModel.Returns(libraryExplorerViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), viewModelHolderStub,
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			target.SelectedSongListIndex = 0;

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(explorerDisc, receivedEvent.Disc);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenPlaylistSongListSelectedAndPlaylistHasCurrentSong_SendsActiveDiscChangedEventForCurrentPlaylistSongDisc()
		{
			// Arrange

			var playlistSongDisc = new Disc();
			var currSong = new Song { Disc = playlistSongDisc };

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns(currSong);
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			target.SelectedSongListIndex = 1;

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(playlistSongDisc, receivedEvent.Disc);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenPlaylistSongListSelectedAndOneDiscPlaylistDoesNotHaveCurrentSong_SendsActiveDiscChangedEventForPlaylistDisc()
		{
			// Arrange

			var playlistDisc = new Disc();

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns((Song)null);
			songPlaylistViewModelStub.PlayedDisc.Returns(playlistDisc);
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			target.SelectedSongListIndex = 1;

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(playlistDisc, receivedEvent.Disc);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenPlaylistSongListSelectedAndMultipleSongsPlaylistDoesNotHaveCurrentSong_SendsActiveDiscChangedEventForNullDisc()
		{
			// Arrange

			ILibraryExplorerViewModel libraryExplorerViewModelStub = Substitute.For<ILibraryExplorerViewModel>();
			libraryExplorerViewModelStub.SelectedDisc.Returns(new Disc());
			IApplicationViewModelHolder viewModelHolderStub = Substitute.For<IApplicationViewModelHolder>();
			viewModelHolderStub.LibraryExplorerViewModel.Returns(libraryExplorerViewModelStub);

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns((Song)null);
			songPlaylistViewModelStub.PlayedDisc.Returns((Disc)null);
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), viewModelHolderStub,
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();
			target.SelectedSongListIndex = 0;

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			target.SelectedSongListIndex = 1;

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.IsNull(receivedEvent.Disc);
		}

		[Test]
		public void LibraryExplorerDiscChangedEventHandler_SwitchesToExplorerSongList()
		{
			// Arrange

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			target.SelectedSongListIndex = 1;

			// Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(new Disc()));

			// Assert

			Assert.AreEqual(0, target.SelectedSongListIndex);
		}

		[Test]
		public void LibraryExplorerDiscChangedEventHandler_SendsActiveDiscChangedEventForCurrentExplorerDisc()
		{
			// Arrange

			var newExplorerDisc = new Disc();

			ILibraryExplorerViewModel libraryExplorerViewModelStub = Substitute.For<ILibraryExplorerViewModel>();
			libraryExplorerViewModelStub.SelectedDisc.Returns(newExplorerDisc);
			IApplicationViewModelHolder viewModelHolderStub = Substitute.For<IApplicationViewModelHolder>();
			viewModelHolderStub.LibraryExplorerViewModel.Returns(libraryExplorerViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), viewModelHolderStub,
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(newExplorerDisc));

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(newExplorerDisc, receivedEvent.Disc);
		}

		[Test]
		public void PlaylistChangedEventHandler_WhenPlaylistSongListSelected_SendsActiveDiscChangedEventForCurrentPlaylistSongDisc()
		{
			// Arrange

			var playlistSongDisc1 = new Disc();
			var playlistSongDisc2 = new Disc();

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns(new Song { Disc = playlistSongDisc1 }, new Song { Disc = playlistSongDisc2 });
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			target.SelectedSongListIndex = 1;

			ActiveDiscChangedEventArgs receivedEvent = null;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = e);

			// Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(playlistSongDisc2, receivedEvent.Disc);
		}

		[Test]
		public void PlaylistChangedEventHandler_WhenExplorerSongListSelected_DoesNotSendActiveDiscChangedEvent()
		{
			// Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns(new Song { Disc = new Disc() });
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			target.SelectedSongListIndex = 0;

			bool receivedEvent = false;
			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => receivedEvent = true);

			// Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.IsFalse(receivedEvent);

			// Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new ActiveDiscChangedEventArgs(null));
		}

		[Test]
		public void PlaylistChangedEventHandler_IfNewPlaylistSongHasArtist_SetsCorrectTitle()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Title = "Some Song",
			};

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSongIndex.Returns(1);
			songPlaylistViewModelStub.SongsNumber.Returns(3);
			songPlaylistViewModelStub.CurrentSong.Returns(song);

			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.AreEqual("2/3 - Some Artist - Some Song", target.Title);
		}

		[Test]
		public void PlaylistChangedEventHandler_IfNewPlaylistSongDoesNotHaveArtist_SetsCorrectTitle()
		{
			// Arrange

			var song = new Song
			{
				Artist = null,
				Title = "Some Song",
			};

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSongIndex.Returns(1);
			songPlaylistViewModelStub.SongsNumber.Returns(3);
			songPlaylistViewModelStub.CurrentSong.Returns(song);

			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.AreEqual("2/3 - Some Song", target.Title);
		}

		[Test]
		public void PlaylistChangedEventHandler_IfPlaylistCurrentSongIsNotSet_SetsDefaultApplicationTitle()
		{
			// Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSong.Returns((Song)null);

			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.AreEqual("Panda Player", target.Title);
		}

		[Test]
		public void PlaylistLoadedEventHandler_SetsCorrectTitle()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Title = "Some Song",
			};

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.CurrentSongIndex.Returns(1);
			songPlaylistViewModelStub.SongsNumber.Returns(3);
			songPlaylistViewModelStub.CurrentSong.Returns(song);

			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaylistLoadedEventArgs(songPlaylistViewModelStub));

			// Assert

			Assert.AreEqual("2/3 - Some Artist - Some Song", target.Title);
		}

		[Test]
		public void PlaySongsListEventHandler_FillsPlaylistWithNewSongs()
		{
			// Arrange

			var songs = new[] { new Song(), new Song() };

			ISongPlaylistViewModel songPlaylistViewModelMock = Substitute.For<ISongPlaylistViewModel>();
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelMock);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			var playSongsListEvent = new PlaySongsListEventArgs(songs);

			// Act

			Messenger.Default.Send(playSongsListEvent);

			// Assert

			Received.InOrder(() =>
			{
				songPlaylistViewModelMock.Received(1).SetSongs(playSongsListEvent.Songs);
				songPlaylistViewModelMock.Received(1).SwitchToNextSong();
			});
		}

		[Test]
		public void PlaySongsListEventHandler_RestartsPlayerPlaybackCorrectly()
		{
			// Arrange

			var songs = new[] { new Song(), new Song() };

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(songs));

			// Assert

			Received.InOrder(() =>
			{
				musicPlayerViewModelMock.Received(1).Stop();
				musicPlayerViewModelMock.Received(1).Play();
			});
		}

		[Test]
		public void PlaySongsListEventHandler_SwitchesToPlaylistView()
		{
			// Arrange

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlaySongsListEventArgs(new[] { new Song() }));

			// Assert

			Assert.AreEqual(1, target.SelectedSongListIndex);
		}

		[Test]
		public void PlayDiscFromSongEventHandler_FillsPlaylistWithDiscSongs()
		{
			// Arrange

			var disc = new Disc();
			var song1 = new Song { Disc = disc };
			var song2 = new Song { Disc = disc };
			disc.SongsUnordered = new[] { song1, song2 };

			List<Song> updatedSongs = null;

			ISongPlaylistViewModel songPlaylistViewModelMock = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelMock.SetSongs(Arg.Do<IEnumerable<Song>>(arg => updatedSongs = arg.ToList()));

			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelMock);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelStub, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			var playDiscFromSongEvent = new PlayDiscFromSongEventArgs(song2);

			// Act

			Messenger.Default.Send(playDiscFromSongEvent);

			// Assert

			Received.InOrder(() =>
			{
				songPlaylistViewModelMock.Received(1).SetSongs(Arg.Any<IEnumerable<Song>>());
				songPlaylistViewModelMock.Received(1).SwitchToSong(song2);
			});
			CollectionAssert.AreEqual(disc.Songs, updatedSongs);
		}

		[Test]
		public void PlayDiscFromSongEventHandler_RestartsPlayerPlaybackCorrectly()
		{
			// Arrange

			var disc = new Disc();
			var song = new Song { Disc = disc };
			disc.SongsUnordered = new[] { song };

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlayDiscFromSongEventArgs(song));

			// Assert

			Received.InOrder(() =>
			{
				musicPlayerViewModelMock.Received(1).Stop();
				musicPlayerViewModelMock.Received(1).Play();
			});
		}

		[Test]
		public void PlayDiscFromSongEventHandler_SwitchesToPlaylistView()
		{
			// Arrange

			var disc = new Disc();
			var song = new Song { Disc = disc };
			disc.SongsUnordered = new[] { song };

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlayDiscFromSongEventArgs(song));

			// Assert

			Assert.AreEqual(1, target.SelectedSongListIndex);
		}

		[Test]
		public void PlayPlaylistStartingFromSongEventHandler_RestartsPlayerPlaybackCorrectly()
		{
			// Arrange

			var song = new Song();

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs(song));

			// Assert

			Received.InOrder(() =>
			{
				musicPlayerViewModelMock.Received(1).Stop();
				musicPlayerViewModelMock.Received(1).Play();
			});
		}

		[Test]
		public void NavigateLibraryExplorerToDiscEventHandler_SwitchesLibraryExplorerToDisc()
		{
			// Arrange

			var disc = new Disc();

			ILibraryExplorerViewModel libraryExplorerViewModelMock = Substitute.For<ILibraryExplorerViewModel>();
			IApplicationViewModelHolder viewModelHolderStub = Substitute.For<IApplicationViewModelHolder>();
			viewModelHolderStub.LibraryExplorerViewModel.Returns(libraryExplorerViewModelMock);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), viewModelHolderStub,
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(disc));

			// Assert

			libraryExplorerViewModelMock.Received(1).SwitchToDisc(disc);
		}

		[Test]
		public void NavigateLibraryExplorerToDiscEventHandler_SwitchesToExplorerSongListView()
		{
			// Arrange

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(new Disc()));

			// Assert

			Assert.AreEqual(0, target.SelectedSongListIndex);
		}

		[Test]
		public void ReversePlayingCommand_IfPlayerIsInPlayingState_PausesPlayback()
		{
			// Arrange

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelMock.IsPlaying.Returns(true);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			target.ReversePlaying();

			// Assert

			musicPlayerViewModelMock.Received(1).Pause();
			musicPlayerViewModelMock.DidNotReceive().Play();
		}

		[Test]
		public void ReversePlayingCommand_IfPlayerIsInPausedState_ResumesPlayback()
		{
			// Arrange

			IMusicPlayerViewModel musicPlayerViewModelMock = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelMock.IsPlaying.Returns(false);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				musicPlayerViewModelMock, Substitute.For<IViewNavigator>());
			target.Load().Wait();

			// Act

			target.ReversePlaying();

			// Assert

			musicPlayerViewModelMock.Received(1).Play();
			musicPlayerViewModelMock.DidNotReceive().Pause();
		}
	}
}
