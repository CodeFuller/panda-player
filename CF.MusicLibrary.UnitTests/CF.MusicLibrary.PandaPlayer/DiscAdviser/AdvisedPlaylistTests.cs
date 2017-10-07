using System;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.DiscAdviser;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	[TestFixture]
	public class AdvisedPlaylistTests
	{
		[Test]
		public void ConstructorFromDisc_IfDiscHasArtist_FillsTitleCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist" } } },
			};

			//	Act

			var target = new AdvisedPlaylist(disc);

			//	Assert

			Assert.AreEqual("Some Artist - Some Disc", target.Title);
		}

		[Test]
		public void ConstructorFromDisc_FillsSongsWithAllActiveDiscSongs()
		{
			//	Arrange

			var activeSong1 = new Song();
			var activeSong2 = new Song();
			var deletedSong = new Song { DeleteDate = new DateTime(2017, 10, 06) };

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { deletedSong, activeSong1, activeSong2 },
			};

			//	Act

			var target = new AdvisedPlaylist(disc);

			//	Assert

			CollectionAssert.AreEqual(new[] {activeSong1, activeSong2}, target.Songs);
		}

		[Test]
		public void ConstructorFromSongList_FillsTitleCorrectly()
		{
			//	Arrange

			//	Act

			var target = new AdvisedPlaylist("Some Title", new[] { new Song() });

			//	Assert

			Assert.AreEqual("Some Title", target.Title);
		}

		[Test]
		public void ConstructorFromSongList_FillsSongsCorrectly()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			//	Act

			var target = new AdvisedPlaylist(String.Empty, new[] { song1, song2 });

			//	Assert

			CollectionAssert.AreEqual(new[] { song1, song2 }, target.Songs);
		}
	}
}
