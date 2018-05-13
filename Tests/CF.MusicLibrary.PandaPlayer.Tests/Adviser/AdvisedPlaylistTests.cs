using System;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser
{
	[TestFixture]
	public class AdvisedPlaylistTests
	{
		[Test]
		public void ForDisc_IfDiscHasArtist_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist" } } },
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreEqual("Some Artist - Some Disc", target.Title);
		}

		[Test]
		public void ForDisc_IfDiscHasNoArtist_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { new Song { Artist = new Artist() }, new Song { Artist = new Artist() } },
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreEqual("Some Disc", target.Title);
		}

		[Test]
		public void ForDisc_FillsSongsWithAllActiveDiscSongs()
		{
			// Arrange

			var activeSong1 = new Song();
			var activeSong2 = new Song();
			var deletedSong = new Song { DeleteDate = new DateTime(2017, 10, 06) };

			var disc = new Disc
			{
				SongsUnordered = new[] { deletedSong, activeSong1, activeSong2 },
			};

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			CollectionAssert.AreEqual(new[] { activeSong1, activeSong2 }, target.Songs);
		}

		[Test]
		public void ForDisc_FillsDiscCorrectly()
		{
			// Arrange

			var disc = new Disc { SongsUnordered = new Song[0] };

			// Act

			var target = AdvisedPlaylist.ForDisc(disc);

			// Assert

			Assert.AreEqual(disc, target.Disc);
		}

		[Test]
		public void ForFavouriteArtistDisc_IfDiscHasArtist_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist" } } },
			};

			// Act

			var target = AdvisedPlaylist.ForFavouriteArtistDisc(disc);

			// Assert

			Assert.AreEqual("*** Some Artist - Some Disc", target.Title);
		}

		[Test]
		public void ForFavouriteArtistDisc_IfDiscHasNoArtist_FillsTitleCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Some Disc",
				SongsUnordered = new[] { new Song { Artist = new Artist() }, new Song { Artist = new Artist() } },
			};

			// Act

			var target = AdvisedPlaylist.ForFavouriteArtistDisc(disc);

			// Assert

			Assert.AreEqual("*** Some Disc", target.Title);
		}

		[Test]
		public void ForFavouriteArtistDisc_FillsSongsWithAllActiveDiscSongs()
		{
			// Arrange

			var activeSong1 = new Song();
			var activeSong2 = new Song();
			var deletedSong = new Song { DeleteDate = new DateTime(2017, 10, 06) };

			var disc = new Disc
			{
				SongsUnordered = new[] { deletedSong, activeSong1, activeSong2 },
			};

			// Act

			var target = AdvisedPlaylist.ForFavouriteArtistDisc(disc);

			// Assert

			CollectionAssert.AreEqual(new[] { activeSong1, activeSong2 }, target.Songs);
		}

		[Test]
		public void ForFavouriteArtistDisc_FillsDiscCorrectly()
		{
			// Arrange

			var disc = new Disc { SongsUnordered = new Song[0] };

			// Act

			var target = AdvisedPlaylist.ForFavouriteArtistDisc(disc);

			// Assert

			Assert.AreEqual(disc, target.Disc);
		}

		[Test]
		public void ForHighlyRatedSongs_FillsTitleCorrectly()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });

			// Assert

			Assert.AreEqual("Highly Rated Songs", target.Title);
		}

		[Test]
		public void ForHighlyRatedSongs_FillsSongsCorrectly()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(new[] { song1, song2 });

			// Assert

			CollectionAssert.AreEqual(new[] { song1, song2 }, target.Songs);
		}

		[Test]
		public void DiscGetter_ForForHighlyRatedSongsPlaylist_ThrowsInvalidOperationException()
		{
			// Arrange

			// Act

			var target = AdvisedPlaylist.ForHighlyRatedSongs(new[] { new Song() });

			// Assert

			Disc disc;
			Assert.Throws<InvalidOperationException>(() => disc = target.Disc);
		}
	}
}
