using System;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class ApplicationViewModelTests
	{
		[Test]
		public void Constructor_IfDiscLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(null, Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfViewModelHolderArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), null,
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfMusicPlayerViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), Substitute.For<IApplicationViewModelHolder>(),
				null, Substitute.For<IViewNavigator>()));
		}

		[Test]
		public void Constructor_IfViewNavigatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(new DiscLibrary(Enumerable.Empty<Disc>()), Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), null));
		}

		[Test]
		public void Load_LoadsDiscLibrary()
		{
			//	Arrange

			bool libraryLoaded = false;
			var library = new DiscLibrary(() =>
			{
				libraryLoaded = true;
				return Task.FromResult(Enumerable.Empty<Disc>());
			});
			var target = new ApplicationViewModel(library, Substitute.For<IApplicationViewModelHolder>(), Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());

			//	Act

			target.Load().Wait();

			//	Assert

			Assert.IsTrue(libraryLoaded);
		}

		[Test]
		public void Load_AfterLibraryIsLoaded_SendsLibraryLoadedEvent()
		{
			//	Arrange

			bool receivedEvent = false;

			var library = new DiscLibrary(() =>
			{
				//	Event should be sent after library loading.
				receivedEvent = false;
				return Task.FromResult(Enumerable.Empty<Disc>());
			});

			Messenger.Default.Register<LibraryLoadedEventArgs>(this, e => receivedEvent = true);
			var target = new ApplicationViewModel(library, Substitute.For<IApplicationViewModelHolder>(), Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());

			//	Act

			target.Load().Wait();

			//	Assert


			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void Load_IfPreviousPlaylistDataWasLoaded_SwitchesToPlaylistView()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.Songs.Returns(new[] { new Song() });
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(), musicPlayerViewModelStub, Substitute.For<IViewNavigator>());

			//	Act

			target.Load().Wait();

			//	Assert

			Assert.AreEqual(1, target.SelectedSongListIndex);
		}

		[Test]
		public void Load_IfPreviousPlaylistDataWasNotLoaded_SwitchesToExplorerSongListView()
		{
			//	Arrange

			ISongPlaylistViewModel songPlaylistViewModelStub = Substitute.For<ISongPlaylistViewModel>();
			songPlaylistViewModelStub.Songs.Returns(Enumerable.Empty<Song>());
			IMusicPlayerViewModel musicPlayerViewModelStub = Substitute.For<IMusicPlayerViewModel>();
			musicPlayerViewModelStub.Playlist.Returns(songPlaylistViewModelStub);

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(), musicPlayerViewModelStub, Substitute.For<IViewNavigator>());

			//	Act

			target.Load().Wait();

			//	Assert

			Assert.AreEqual(0, target.SelectedSongListIndex);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenExplorerSongListSelected_SendsActiveDiscChangedEventForCurrentExplorerDisc()
		{
			//	Arrange

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

			//	Act

			target.SelectedSongListIndex = 0;

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(explorerDisc, receivedEvent.Disc);
		}

		[Test]
		public void SelectedSongListIndexSetter_WhenPlaylistSongListSelected_SendsActiveDiscChangedEventForCurrentPlaylistSongDisc()
		{
			//	Arrange

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

			//	Act

			target.SelectedSongListIndex = 1;

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(playlistSongDisc, receivedEvent.Disc);
		}

		[Test]
		public void LibraryExplorerDiscChangedEventHandler_SwitchesToExplorerSongList()
		{
			//	Arrange

			var target = new ApplicationViewModel(new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>())), Substitute.For<IApplicationViewModelHolder>(),
				Substitute.For<IMusicPlayerViewModel>(), Substitute.For<IViewNavigator>());
			target.Load().Wait();

			target.SelectedSongListIndex = 1;

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(new Disc()));

			//	Assert

			Assert.AreEqual(0, target.SelectedSongListIndex);
		}

		[Test]
		public void LibraryExplorerDiscChangedEventHandler_SendsActiveDiscChangedEventForCurrentExplorerDisc()
		{
			//	Arrange

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

			//	Act

			Messenger.Default.Send(new LibraryExplorerDiscChangedEventArgs(newExplorerDisc));

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(newExplorerDisc, receivedEvent.Disc);
		}

		[Test]
		public void PlaylistChangedEventHandler_WhenPlaylistSongListSelected_SendsActiveDiscChangedEventForCurrentPlaylistSongDisc()
		{
			//	Arrange

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

			//	Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			//	Assert

			Assert.IsNotNull(receivedEvent);
			Assert.AreSame(playlistSongDisc2, receivedEvent.Disc);
		}

		[Test]
		public void PlaylistChangedEventHandler_WhenExplorerSongListSelected_DoesNotSendActiveDiscChangedEvent()
		{
			//	Arrange

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

			//	Act

			Messenger.Default.Send(new PlaylistChangedEventArgs(songPlaylistViewModelStub));

			//	Assert

			Assert.IsFalse(receivedEvent);
		}
	}
}
