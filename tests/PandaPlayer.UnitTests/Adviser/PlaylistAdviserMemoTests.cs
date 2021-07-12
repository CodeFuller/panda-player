using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Adviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.UnitTests.Adviser
{
	[TestClass]
	public class PlaylistAdviserMemoTests
	{
		[TestMethod]
		public void RegisterPlayback_IfPlaybackAdviseIsFavoriteArtistDisc_SetsPlaybacksSinceFavoriteArtistDiscToZero()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>(),
			};

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForFavoriteArtistDisc(disc));

			// Assert

			Assert.AreEqual(0, newMemo.PlaybacksSinceFavoriteArtistDisc);
			Assert.AreEqual(4, newMemo.PlaybacksSinceHighlyRatedSongsPlaylist);
		}

		[TestMethod]
		public void RegisterPlayback_IfPlaybackAdviseIsHighlyRatedSongs_SetsPlaybacksSinceHighlyRatedSongsPlaylistToZero()
		{
			// Arrange

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>()));

			// Assert

			Assert.AreEqual(0, newMemo.PlaybacksSinceHighlyRatedSongsPlaylist);
			Assert.AreEqual(6, newMemo.PlaybacksSinceFavoriteArtistDisc);
		}

		[TestMethod]
		public void RegisterPlayback_IfPlaybackIsNotSpeciallyTracked_IncrementsAllPlaybacksCounters()
		{
			// Arrange

			var disc = new DiscModel
			{
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>(),
			};

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForDisc(disc));

			// Assert

			Assert.AreEqual(4, newMemo.PlaybacksSinceHighlyRatedSongsPlaylist);
			Assert.AreEqual(6, newMemo.PlaybacksSinceFavoriteArtistDisc);
		}
	}
}
