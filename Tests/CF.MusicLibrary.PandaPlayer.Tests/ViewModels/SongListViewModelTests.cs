using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CF.Library.Core.Attributes;
using CF.Library.Core.Enums;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.ViewModels
{
	[TestFixture]
	public class SongListViewModelTests
	{
		[ExcludeFromTestCoverage("Empty stub of base abstract class")]
		private class ConcreteSongListViewModel : SongListViewModel
		{
			public int OnSongItemsChangedCallsNumber { get; set; }

			public ConcreteSongListViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
				: base(libraryContentUpdater, viewNavigator, windowService)
			{
			}

			public override bool DisplayTrackNumbers => true;

			public override ICommand PlayFromSongCommand { get; }

			protected override void OnSongItemsChanged()
			{
				++OnSongItemsChangedCallsNumber;
				base.OnSongItemsChanged();
			}

			public void InvokeAddSongs(IEnumerable<Song> addedSongs)
			{
				AddSongs(addedSongs);
			}

			public void InvokeInsertSongs(int index, IEnumerable<Song> addedSongs)
			{
				InsertSongs(index, addedSongs);
			}
		}

		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void SetSongs_IfCurrentSongListIsEmpty_SetsSongsToNewList()
		{
			// Arrange

			List<Song> newSongList = new List<Song> { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			// Act

			target.SetSongs(newSongList);

			// Assert

			CollectionAssert.AreEqual(newSongList, target.Songs);
		}

		[Test]
		public void SetSongs_IfCurrentSongListIsNotEmpty_ClearsPreviousSongList()
		{
			// Arrange

			List<Song> oldSongList = new List<Song> { new Song(), new Song() };
			List<Song> newSongList = new List<Song> { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(oldSongList);

			// Act

			target.SetSongs(newSongList);

			// Assert

			CollectionAssert.AreEqual(newSongList, target.Songs);
		}

		[Test]
		public void SetSongs_SendsPropertyChangedEventsForAffectedProperties()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			// Act

			target.SetSongs(new[] { new Song() });

			// Assert

			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.HasSongs));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.SongsNumber));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.TotalSongsFileSize));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.TotalSongsDuration));
		}

		[Test]
		public void SetSongs_CallsOnSongItemsChangedOnce()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			// Act

			target.SetSongs(new[] { new Song(), new Song() });

			// Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void AddSongs_AddsSongsToSongList()
		{
			// Arrange

			var oldSongs = new[] { new Song(), new Song() };
			var newSongs = new[] { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(oldSongs);

			// Act

			target.InvokeAddSongs(newSongs);

			// Assert

			CollectionAssert.AreEqual(oldSongs.Concat(newSongs), target.Songs);
		}

		[Test]
		public void AddSongs_CallsOnSongItemsChangedOnce()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song(), new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			// Act

			target.InvokeAddSongs(new[] { new Song(), new Song() });

			// Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void InsertSongs_InsertsSongsCorrectly()
		{
			// Arrange

			var oldSong1 = new Song();
			var oldSong2 = new Song();
			var newSong1 = new Song();
			var newSong2 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { oldSong1, oldSong2 });

			// Act

			target.InvokeInsertSongs(1, new[] { newSong1, newSong2 });

			// Assert

			CollectionAssert.AreEqual(new[] { oldSong1, newSong1, newSong2, oldSong2 }, target.Songs);
		}

		[Test]
		public void InsertSongs_CallsOnSongItemsChangedOnce()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song(), new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			// Act

			target.InvokeInsertSongs(1, new[] { new Song(), new Song() });

			// Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void PlaySongsNextCommand_IfSomeSongsAreSelected_SendsAddingSongsToPlaylistNextEventWithSelectedSongs()
		{
			// Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			AddingSongsToPlaylistNextEventArgs receivedEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => receivedEvent = e);

			// Act

			target.PlaySongsNext();

			// Assert

			Assert.IsNotNull(receivedEvent);
			CollectionAssert.AreEqual(new[] { song1, song2 }, receivedEvent.Songs);
		}

		[Test]
		public void PlaySongsNextCommand_IfNoSongsAreSelected_ReturnsWithNoAction()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song() });
			target.SelectedSongItems = Array.Empty<SongListItem>();

			bool receivedEvent = false;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => receivedEvent = true);

			// Act

			target.PlaySongsNext();

			// Assert

			Assert.IsFalse(receivedEvent);

			// Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(Array.Empty<Song>()));
		}

		[Test]
		public void PlaySongsLastCommand_IfSomeSongsAreSelected_SendsAddingSongsToPlaylistLastEventWithSelectedSongs()
		{
			// Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			AddingSongsToPlaylistLastEventArgs receivedEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => receivedEvent = e);

			// Act

			target.PlaySongsLast();

			// Assert

			Assert.IsNotNull(receivedEvent);
			CollectionAssert.AreEqual(new[] { song1, song2 }, receivedEvent.Songs);
		}

		[Test]
		public void PlaySongsLastCommand_IfNoSongsAreSelected_ReturnsWithNoAction()
		{
			// Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), Substitute.For<IWindowService>());
			target.SetSongs(new[] { new Song() });
			target.SelectedSongItems = Array.Empty<SongListItem>();

			bool receivedEvent = false;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => receivedEvent = true);

			// Act

			target.PlaySongsLast();

			// Assert

			Assert.IsFalse(receivedEvent);

			// Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(Array.Empty<Song>()));
		}

		[Test]
		public void DeleteSongsFromDiscCommand_IfNoSongsAreSelected_ReturnsWithNoAction()
		{
			// Arrange

			var windowServiceMock = Substitute.For<IWindowService>();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>(), windowServiceMock);
			target.SetSongs(new[] { new Song() });
			target.SelectedSongItems = Array.Empty<SongListItem>();

			// Act

			target.DeleteSongsFromDisc().Wait();

			// Assert

			windowServiceMock.DidNotReceive().ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>());
		}

		[Test]
		public void DeleteSongsFromDiscCommand_IfDeletionIsNotConfirmed_ReturnsWithNoAction()
		{
			// Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.No);

			var contentUpdaterMock = Substitute.For<ILibraryContentUpdater>();

			var target = new ConcreteSongListViewModel(contentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			// Act

			target.DeleteSongsFromDisc().Wait();

			// Assert

			contentUpdaterMock.DidNotReceive().DeleteSong(Arg.Any<Song>());
		}

		[Test]
		public void DeleteSongsFromDiscCommand_IfDeletionIsConfirmed_DeletesSongsCorrectly()
		{
			// Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ShowMessageBoxButton>(), Arg.Any<ShowMessageBoxIcon>()).Returns(ShowMessageBoxResult.Yes);

			var contentUpdaterMock = Substitute.For<ILibraryContentUpdater>();

			var target = new ConcreteSongListViewModel(contentUpdaterMock, Substitute.For<IViewNavigator>(), windowServiceStub);
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			// Act

			target.DeleteSongsFromDisc().Wait();

			// Assert

			contentUpdaterMock.Received(2).DeleteSong(Arg.Any<Song>());
			contentUpdaterMock.Received(1).DeleteSong(song1);
			contentUpdaterMock.Received(1).DeleteSong(song2);
		}
	}
}
