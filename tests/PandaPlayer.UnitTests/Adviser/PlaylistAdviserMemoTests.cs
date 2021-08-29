using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PandaPlayer.Adviser;
using PandaPlayer.Core.Models;
using PandaPlayer.UnitTests.Extensions;

namespace PandaPlayer.UnitTests.Adviser
{
	[TestClass]
	public class PlaylistAdviserMemoTests
	{
		[TestMethod]
		public void RegisterPlayback_IfPlaybackAdviseIsFavoriteArtistAdviseSet_SetsPlaybacksSinceFavoriteArtistDiscToZero()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>(),
			};

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForFavoriteArtistAdviseSet(disc.ToAdviseSet()));

			// Assert

			var expectedMemo = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 4, playbacksSinceFavoriteArtistDisc: 0);
			newMemo.Should().BeEquivalentTo(expectedMemo);
		}

		[TestMethod]
		public void RegisterPlayback_IfPlaybackAdviseIsHighlyRatedSongs_SetsPlaybacksSinceHighlyRatedSongsPlaylistToZero()
		{
			// Arrange

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForHighlyRatedSongs(Enumerable.Empty<SongModel>()));

			// Assert

			var expectedMemo = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 0, playbacksSinceFavoriteArtistDisc: 6);
			newMemo.Should().BeEquivalentTo(expectedMemo);
		}

		[TestMethod]
		public void RegisterPlayback_IfPlaybackIsNotSpeciallyTracked_IncrementsAllPlaybacksCounters()
		{
			// Arrange

			var disc = new DiscModel
			{
				Id = new ItemId("1"),
				Folder = new ShallowFolderModel(),
				AllSongs = new List<SongModel>(),
			};

			var target = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 3, playbacksSinceFavoriteArtistDisc: 5);

			// Act

			var newMemo = target.RegisterPlayback(AdvisedPlaylist.ForAdviseSet(disc.ToAdviseSet()));

			// Assert

			var expectedMemo = new PlaylistAdviserMemo(playbacksSinceHighlyRatedSongsPlaylist: 4, playbacksSinceFavoriteArtistDisc: 6);
			newMemo.Should().BeEquivalentTo(expectedMemo);
		}
	}
}
