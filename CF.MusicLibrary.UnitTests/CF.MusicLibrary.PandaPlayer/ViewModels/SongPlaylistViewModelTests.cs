using System.Collections.Generic;
using System.Linq;
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
	public class SongPlaylistViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[Test]
		public void SetSongs_WhenSongListIsUpdated_SendsPlaylistChangedEventWithUpdatedSongs()
		{
			//	Arrange

			var songs = new List<Song> { new Song() };
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			List<Song> registeredSongs = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredSongs = e.Playlist.Songs.ToList());

			//	Act

			target.SetSongs(songs);

			//	Assert

			Assert.IsNotNull(registeredSongs);
			CollectionAssert.AreEqual(songs, registeredSongs);
		}

		[Test]
		public void SetSongs_WhenSwitchingToNextSong_SendsPlaylistChangedEventWithNewCurrentSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });
			target.SwitchToNextSong();

			Song registeredCurrentSong = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredCurrentSong = e.Playlist.CurrentSong);

			//	Act

			target.SwitchToNextSong();

			//	Assert

			Assert.IsNotNull(registeredCurrentSong);
			Assert.AreSame(song2, registeredCurrentSong);
		}

		[Test]
		public void SetSongs_WhenSwitchingToSpecifiedSong_SendsPlaylistChangedEventWithNewCurrentSong()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();
			var target = new SongPlaylistViewModel(Substitute.For<ILibraryContentUpdater>(), Substitute.For<IViewNavigator>());

			target.SetSongs(new List<Song> { song1, song2 });

			Song registeredCurrentSong = null;
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => registeredCurrentSong = e.Playlist.CurrentSong);

			//	Act

			target.SwitchToSong(song2);

			//	Assert

			Assert.IsNotNull(registeredCurrentSong);
			Assert.AreSame(song2, registeredCurrentSong);
		}
	}
}
