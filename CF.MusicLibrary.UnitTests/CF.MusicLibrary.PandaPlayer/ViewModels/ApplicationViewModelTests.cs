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
	}
}
