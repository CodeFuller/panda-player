using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CF.Library.Core.Attributes;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels
{
	[TestFixture]
	public class SongListViewModelTests
	{
		[ExcludeFromTestCoverage("Empty stub of base abstract class")]
		private class ConcreteSongListViewModel : SongListViewModel
		{
			public int OnSongItemsChangedCallsNumber { get; set; }

			public ConcreteSongListViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
				: base(libraryContentUpdater, viewNavigator)
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
			//	Arrange

			List<Song> newSongList = new List<Song> { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			//	Act

			target.SetSongs(newSongList);

			//	Assert

			CollectionAssert.AreEqual(newSongList, target.Songs);
		}

		[Test]
		public void SetSongs_IfCurrentSongListIsNotEmpty_ClearsPreviousSongList()
		{
			//	Arrange

			List<Song> oldSongList = new List<Song> { new Song(), new Song() };
			List<Song> newSongList = new List<Song> { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(oldSongList);

			//	Act

			target.SetSongs(newSongList);

			//	Assert

			CollectionAssert.AreEqual(newSongList, target.Songs);
		}

		[Test]
		public void SetSongs_SendsPropertyChangedEventsForAffectedProperties()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			//	Act

			target.SetSongs(new[] { new Song() });

			//	Assert

			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.HasSongs));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.SongsNumber));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.TotalSongsFileSize));
			CollectionAssert.Contains(changedProperties, nameof(SongListViewModel.TotalSongsDuration));
		}

		[Test]
		public void SetSongs_CallsOnSongItemsChangedOnce()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			//	Act

			target.SetSongs(new[] { new Song(), new Song() });

			//	Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void AddSongs_AddsSongsToSongList()
		{
			//	Arrange

			var oldSongs = new[] { new Song(), new Song() };
			var newSongs = new[] { new Song(), new Song() };

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(oldSongs);

			//	Act

			target.InvokeAddSongs(newSongs);

			//	Assert

			CollectionAssert.AreEqual(oldSongs.Union(newSongs), target.Songs);
		}

		[Test]
		public void AddSongs_CallsOnSongItemsChangedOnce()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { new Song(), new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			//	Act

			target.InvokeAddSongs(new[] { new Song(), new Song() });

			//	Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void InsertSongs_InsertsSongsCorrectly()
		{
			//	Arrange

			var oldSong1 = new Song();
			var oldSong2 = new Song();
			var newSong1 = new Song();
			var newSong2 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { oldSong1, oldSong2 });

			//	Act

			target.InvokeInsertSongs(1, new[] { newSong1, newSong2 });

			//	Assert

			CollectionAssert.AreEqual(new[] { oldSong1, newSong1, newSong2, oldSong2 }, target.Songs);
		}

		[Test]
		public void InsertSongs_CallsOnSongItemsChangedOnce()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { new Song(), new Song() });
			target.OnSongItemsChangedCallsNumber = 0;

			//	Act

			target.InvokeInsertSongs(1, new[] { new Song(), new Song() });

			//	Assert

			Assert.AreEqual(1, target.OnSongItemsChangedCallsNumber);
		}

		[Test]
		public void PlaySongsNextCommand_IfSomeSongsAreSelected_SendsAddingSongsToPlaylistNextEventWithSelectedSongs()
		{
			//	Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			AddingSongsToPlaylistNextEventArgs receivedEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlaySongsNext();

			//	Assert

			Assert.IsNotNull(receivedEvent);
			CollectionAssert.AreEqual(new[] { song1, song2 }, receivedEvent.Songs);
		}

		[Test]
		public void PlaySongsNextCommand_IfNoSongsAreSelected_ReturnsWithNoAction()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { new Song() });
			target.SelectedSongItems = new SongListItem[0];

			bool receivedEvent = false;
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => receivedEvent = true);

			//	Act

			target.PlaySongsNext();

			//	Assert

			Assert.IsFalse(receivedEvent);
			//	Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(new Song[0]));
		}

		[Test]
		public void PlaySongsLastCommand_IfSomeSongsAreSelected_SendsAddingSongsToPlaylistLastEventWithSelectedSongs()
		{
			//	Arrange

			Song song1 = new Song();
			Song song2 = new Song();
			Song song3 = new Song();

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { song1, song2, song3 });
			target.SelectedSongItems = new[] { new SongListItem(song1), new SongListItem(song2) };

			AddingSongsToPlaylistLastEventArgs receivedEvent = null;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => receivedEvent = e);

			//	Act

			target.PlaySongsLast();

			//	Assert

			Assert.IsNotNull(receivedEvent);
			CollectionAssert.AreEqual(new[] { song1, song2 }, receivedEvent.Songs);
		}

		[Test]
		public void PlaySongsLastCommand_IfNoSongsAreSelected_ReturnsWithNoAction()
		{
			//	Arrange

			var target = new ConcreteSongListViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());
			target.SetSongs(new[] { new Song() });
			target.SelectedSongItems = new SongListItem[0];

			bool receivedEvent = false;
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => receivedEvent = true);

			//	Act

			target.PlaySongsLast();

			//	Assert

			Assert.IsFalse(receivedEvent);
			//	Avoiding uncovered lambda code (receivedEvent = true)
			Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(new Song[0]));
		}
	}
}
