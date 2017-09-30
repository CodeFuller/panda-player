using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.BL.Objects
{
	[TestFixture]
	public class DiscTests
	{
		[Test]
		public void ArtistGetter_WhenAllSongsHaveSameArtist_ReturnsThisArtist()
		{
			//	Arrange

			var artist = new Artist();

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = artist },
					new Song { Artist = artist },
				}
			};

			//	Act

			var returnedArtist = disc.Artist;

			//	Assert

			Assert.AreSame(artist, returnedArtist);
		}

		[Test]
		public void ArtistGetter_WhenSongsHaveDifferentArtists_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist() },
					new Song { Artist = new Artist() },
				}
			};

			//	Act

			var returnedArtist = disc.Artist;

			//	Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void ArtistGetter_WhenSomeSongsHaveNullArtist_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist() },
					new Song { Artist = null },
				}
			};

			//	Act

			var returnedArtist = disc.Artist;

			//	Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void GenreGetter_WhenAllSongsHaveSameGenre_ReturnsThisGenre()
		{
			//	Arrange

			var Genre = new Genre();

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = Genre },
					new Song { Genre = Genre },
				}
			};

			//	Act

			var returnedGenre = disc.Genre;

			//	Assert

			Assert.AreSame(Genre, returnedGenre);
		}

		[Test]
		public void GenreGetter_WhenSongsHaveDifferentGenres_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = new Genre() },
					new Song { Genre = new Genre() },
				}
			};

			//	Act

			var returnedGenre = disc.Genre;

			//	Assert

			Assert.IsNull(returnedGenre);
		}

		[Test]
		public void GenreGetter_WhenSomeSongsHaveNullGenre_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = new Genre() },
					new Song { Genre = null },
				}
			};

			//	Act

			var returnedGenre = disc.Artist;

			//	Assert

			Assert.IsNull(returnedGenre);
		}

		[Test]
		public void YearGetter_WhenAllSongsHaveSameYear_ReturnsThisYear()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = 2017 },
					new Song { Year = 2017 },
				}
			};

			//	Act

			var returnedYear = disc.Year;

			//	Assert

			Assert.AreEqual(2017, returnedYear);
		}

		[Test]
		public void YearGetter_WhenSongsHaveDifferentYears_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = 2016 },
					new Song { Year = 2017 },
				}
			};

			//	Act

			var returnedYear = disc.Year;

			//	Assert

			Assert.IsNull(returnedYear);
		}

		[Test]
		public void YearGetter_WhenSomeSongsHaveNullYear_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = null },
					new Song { Year = 2017 },
				}
			};

			//	Act

			var returnedYear = disc.Year;

			//	Assert

			Assert.IsNull(returnedYear);
		}

		[Test]
		public void LastPlaybackTimeGetter_WhenAllSongsHaveLastPlaybackTime_ReturnsEarliestSongLastPlaybackTime()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_WhenSomeSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = null, },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfDiscIsDeleted_ReturnsEarliestSongLastPlaybackTime()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), DeleteDate = new DateTime(2017, 12, 01) },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfDiscIsDeletedAndSomeSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = null, DeleteDate = new DateTime(2017, 12, 01) },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfSomeSongsAreDeleted_ReturnsEarliestSongLastPlaybackTimeAmongActiveSongs()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 01, 01), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfSomeSongsAreDeletedAndSomeActiveSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 01, 01), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = null, },
				}
			};

			//	Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			//	Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void SongsGetter_ReturnsAllActiveSongs()
		{
			//	Arrange

			var song1 = new Song();
			var song2 = new Song();

			var disc = new Disc { SongsUnordered = new[] { song1, song2 } };

			//	Act

			var songs = disc.Songs;

			//	Assert

			CollectionAssert.AreEqual(new[] { song1, song2 }, songs);
		}

		[Test]
		public void SongsGetter_DoesNotReturnDeletedSongs()
		{
			//	Arrange

			var disc = new Disc { SongsUnordered = new[] { new Song { DeleteDate = new DateTime(2017, 09, 30) } } };

			//	Act & Assert

			Assert.IsEmpty(disc.Songs);
		}

		[Test]
		public void IsDeletedGetter_WhenSomeSongsAreNotDeleted_ReturnsFalse()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { DeleteDate = null},
					new Song { DeleteDate = new DateTime(2017, 09, 30)},
				}
			};

			//	Act & Assert

			Assert.IsFalse(disc.IsDeleted);
		}

		[Test]
		public void IsDeletedGetter_WhenAllSongsAreDeleted_ReturnsTrue()
		{
			//	Arrange

			var disc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { DeleteDate = new DateTime(2017, 09, 30)},
					new Song { DeleteDate = new DateTime(2017, 09, 30)},
				}
			};

			//	Act & Assert

			Assert.IsTrue(disc.IsDeleted);
		}
	}
}
