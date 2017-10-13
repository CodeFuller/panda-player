using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.Adviser
{
	[TestFixture]
	public class PlaylistAdviserMemoTests
	{
		[Test]
		public void RegisterPlayback_IfPlaybackAdviseIsFavouriteArtistDisc_SetsPlaybacksSinceFavouriteArtistDiscToZero()
		{
			//	Arrange

			var target = new PlaylistAdviserMemo
			{
				PlaybacksSinceFavouriteArtistDisc = 5,
			};

			//	Act

			target.RegisterPlayback(AdvisedPlaylist.ForFavouriteArtistDisc(new Disc()));

			//	Assert

			Assert.AreEqual(0, target.PlaybacksSinceFavouriteArtistDisc);
		}

		[Test]
		public void RegisterPlayback_IfPlaybackAdviseIsHighlyRatedSongs_SetsPlaybacksSinceHighlyRatedSongsPlaylistToZero()
		{
			//	Arrange

			var target = new PlaylistAdviserMemo
			{
				PlaybacksSinceFavouriteArtistDisc = 5,
			};

			//	Act

			target.RegisterPlayback(AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() }));

			//	Assert

			Assert.AreEqual(0, target.PlaybacksSinceHighlyRatedSongsPlaylist);
		}

		[Test]
		public void RegisterPlayback_IfPlaybackIsNotSpeciallyTracked_IncrementsAllPlaybacksCounters()
		{
			//	Arrange

			var target = new PlaylistAdviserMemo
			{
				PlaybacksSinceHighlyRatedSongsPlaylist = 7,
				PlaybacksSinceFavouriteArtistDisc = 5,
			};

			//	Act

			target.RegisterPlayback(AdvisedPlaylist.ForDisc(new Disc()));

			//	Assert

			Assert.AreEqual(8, target.PlaybacksSinceHighlyRatedSongsPlaylist);
			Assert.AreEqual(6, target.PlaybacksSinceFavouriteArtistDisc);
		}

		[Test]
		public void Clone_CreatesExactObjectCopy()
		{
			//	Arrange

			var target = new PlaylistAdviserMemo
			{
				PlaybacksSinceHighlyRatedSongsPlaylist = 7,
				PlaybacksSinceFavouriteArtistDisc = 5,
			};

			//	Act

			var copy = target.Clone();

			//	Assert

			var memoCopy = copy as PlaylistAdviserMemo;
			Assert.IsNotNull(memoCopy);
			Assert.AreEqual(7, memoCopy.PlaybacksSinceHighlyRatedSongsPlaylist);
			Assert.AreEqual(5, memoCopy.PlaybacksSinceFavouriteArtistDisc);
		}
	}
}
