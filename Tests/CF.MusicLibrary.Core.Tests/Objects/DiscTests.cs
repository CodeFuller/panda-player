using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using NUnit.Framework;

namespace CF.MusicLibrary.Core.Tests.Objects
{
	[TestFixture]
	public class DiscTests
	{
		[Test]
		public void ArtistGetter_ForActiveDiscWhenAllSongsHaveSameArtist_ReturnsThisArtist()
		{
			// Arrange

			var artist = new Artist();

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = artist },
					new Song { Artist = artist },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.AreSame(artist, returnedArtist);
		}

		[Test]
		public void ArtistGetter_ForActiveDiscWhenSongsHaveDifferentArtists_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist() },
					new Song { Artist = new Artist() },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void ArtistGetter_ForActiveDiscWhenSomeSongsHaveNullArtist_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist() },
					new Song { Artist = null },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void ArtistGetter_ForDeletedDiscWhenAllSongsHaveSameArtist_ReturnsThisArtist()
		{
			// Arrange

			var artist = new Artist();

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = artist, DeleteDate = new DateTime(2018, 09, 30) },
					new Song { Artist = artist, DeleteDate = new DateTime(2018, 09, 30) },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.AreSame(artist, returnedArtist);
		}

		[Test]
		public void ArtistGetter_ForDeletedDiscWhenSongsHaveDifferentArtists_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist(), DeleteDate = new DateTime(2018, 09, 30) },
					new Song { Artist = new Artist(), DeleteDate = new DateTime(2018, 09, 30) },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void ArtistGetter_ForDeletedDiscWhenSomeSongsHaveNullArtist_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Artist = new Artist(), DeleteDate = new DateTime(2018, 09, 30) },
					new Song { Artist = null, DeleteDate = new DateTime(2018, 09, 30) },
				}
			};

			// Act

			var returnedArtist = disc.Artist;

			// Assert

			Assert.IsNull(returnedArtist);
		}

		[Test]
		public void GenreGetter_WhenAllSongsHaveSameGenre_ReturnsThisGenre()
		{
			// Arrange

			var genre = new Genre();

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = genre },
					new Song { Genre = genre },
				}
			};

			// Act

			var returnedGenre = disc.Genre;

			// Assert

			Assert.AreSame(genre, returnedGenre);
		}

		[Test]
		public void GenreGetter_WhenSongsHaveDifferentGenres_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = new Genre() },
					new Song { Genre = new Genre() },
				}
			};

			// Act

			var returnedGenre = disc.Genre;

			// Assert

			Assert.IsNull(returnedGenre);
		}

		[Test]
		public void GenreGetter_WhenSomeSongsHaveNullGenre_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Genre = new Genre() },
					new Song { Genre = null },
				}
			};

			// Act

			var returnedGenre = disc.Artist;

			// Assert

			Assert.IsNull(returnedGenre);
		}

		[Test]
		public void YearGetter_WhenAllSongsHaveSameYear_ReturnsThisYear()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = 2017 },
					new Song { Year = 2017 },
				}
			};

			// Act

			var returnedYear = disc.Year;

			// Assert

			Assert.AreEqual(2017, returnedYear);
		}

		[Test]
		public void YearGetter_WhenSongsHaveDifferentYears_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = 2016 },
					new Song { Year = 2017 },
				}
			};

			// Act

			var returnedYear = disc.Year;

			// Assert

			Assert.IsNull(returnedYear);
		}

		[Test]
		public void YearGetter_WhenSomeSongsHaveNullYear_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { Year = null },
					new Song { Year = 2017 },
				}
			};

			// Act

			var returnedYear = disc.Year;

			// Assert

			Assert.IsNull(returnedYear);
		}

		[Test]
		public void LastPlaybackTimeGetter_WhenAllSongsHaveLastPlaybackTime_ReturnsEarliestSongLastPlaybackTime()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), },
				}
			};

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_WhenSomeSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = null, },
				}
			};

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfDiscIsDeleted_ReturnsEarliestSongLastPlaybackTime()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 11, 28), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 05, 09), DeleteDate = new DateTime(2017, 12, 01) },
				}
			};

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfDiscIsDeletedAndSomeSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = null, DeleteDate = new DateTime(2017, 12, 01) },
				}
			};

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfSomeSongsAreDeleted_ReturnsEarliestSongLastPlaybackTimeAmongActiveSongs()
		{
			// Arrange

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

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.AreEqual(new DateTime(2017, 04, 03), lastPlaybackTime);
		}

		[Test]
		public void LastPlaybackTimeGetter_IfSomeSongsAreDeletedAndSomeActiveSongsHaveNoLastPlaybackTime_ReturnsNull()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new List<Song>
				{
					new Song { LastPlaybackTime = new DateTime(2017, 01, 01), DeleteDate = new DateTime(2017, 12, 01) },
					new Song { LastPlaybackTime = new DateTime(2017, 04, 03), },
					new Song { LastPlaybackTime = null, },
				}
			};

			// Act

			var lastPlaybackTime = disc.LastPlaybackTime;

			// Assert

			Assert.IsNull(lastPlaybackTime);
		}

		[Test]
		public void SongsGetter_ReturnsAllActiveSongs()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();

			var disc = new Disc { SongsUnordered = new[] { song1, song2 } };

			// Act

			var songs = disc.Songs;

			// Assert

			CollectionAssert.AreEqual(new[] { song1, song2 }, songs);
		}

		[Test]
		public void SongsGetter_DoesNotReturnDeletedSongs()
		{
			// Arrange

			var disc = new Disc { SongsUnordered = new[] { new Song { DeleteDate = new DateTime(2017, 09, 30) } } };

			// Act & Assert

			Assert.IsEmpty(disc.Songs);
		}

		[Test]
		public void SongsGetter_IfAllSongsHaveTrackNumbers_OrdersSongsByTrackNumber()
		{
			// Arrange

			var song1 = new Song
			{
				TrackNumber = 1,
				Artist = new Artist { Name = "Artist 3" },
				Title = "Title 3",
			};

			var song2 = new Song
			{
				TrackNumber = 2,
				Artist = new Artist { Name = "Artist 1" },
				Title = "Title 1",
			};

			var song3 = new Song
			{
				TrackNumber = 3,
				Artist = new Artist { Name = "Artist 2" },
				Title = "Title 2",
			};

			var disc = new Disc { SongsUnordered = new[] { song3, song2, song1 } };

			// Act & Assert

			CollectionAssert.AreEqual(new[] { song1, song2, song3 }, disc.Songs);
		}

		[Test]
		public void SongsGetter_IfSomeSongsDoNotHaveTrackNumber_ReturnsThemAtTheTailOrderedByArtistName()
		{
			// Arrange

			var song1 = new Song
			{
				TrackNumber = 1,
				Artist = new Artist { Name = "Artist 3" },
				Title = "Title 3",
			};

			var song2 = new Song
			{
				TrackNumber = null,
				Artist = new Artist { Name = "Artist 1" },
				Title = "Title 2",
			};

			var song3 = new Song
			{
				TrackNumber = null,
				Artist = new Artist { Name = "Artist 2" },
				Title = "Title 1",
			};

			var disc = new Disc { SongsUnordered = new[] { song3, song2, song1 } };

			// Act & Assert

			CollectionAssert.AreEqual(new[] { song1, song2, song3 }, disc.Songs);
		}

		[Test]
		public void SongsGetter_IfSomeSongsDoNotHaveTrackNumberAndArtist_ReturnsThemAtTheTailOrderedByTitle()
		{
			// Arrange

			var song1 = new Song
			{
				TrackNumber = 1,
				Artist = new Artist { Name = "Artist 2" },
				Title = "Title 4",
			};

			var song2 = new Song
			{
				TrackNumber = null,
				Artist = new Artist { Name = "Artist 1" },
				Title = "Title 3",
			};

			var song3 = new Song
			{
				TrackNumber = null,
				Artist = null,
				Title = "Title 1",
			};

			var song4 = new Song
			{
				TrackNumber = null,
				Artist = null,
				Title = "Title 2",
			};

			var disc = new Disc { SongsUnordered = new[] { song4, song3, song2, song1 } };

			// Act & Assert

			CollectionAssert.AreEqual(new[] { song1, song2, song3, song4 }, disc.Songs);
		}

		[Test]
		public void CoverImageGetter_IfDiscContainsCoverImage_ReturnsThisImage()
		{
			// Arrange

			var otherImage = new DiscImage { ImageType = DiscImageType.None };
			var coverImage = new DiscImage { ImageType = DiscImageType.Cover };
			var target = new Disc
			{
				Images = { otherImage, coverImage },
			};

			// Act & Assert

			Assert.AreSame(coverImage, target.CoverImage);
		}

		[Test]
		public void CoverImageGetter_IfDiscContainsNoCoverImage_ReturnsNull()
		{
			// Arrange

			var target = new Disc
			{
				Images = { new DiscImage { ImageType = DiscImageType.None } },
			};

			// Act & Assert

			Assert.IsNull(target.CoverImage);
		}

		[Test]
		public void CoverImageSetter_IfDiscContainsCoverImage_ReplacesCoverImageWithNewValue()
		{
			// Arrange

			var coverImage = new DiscImage { ImageType = DiscImageType.Cover };
			var newCoverImage = new DiscImage { ImageType = DiscImageType.Cover };

			var target = new Disc
			{
				Images =
				{
					new DiscImage { ImageType = DiscImageType.None },
					coverImage,
				},
			};

			// Act

			target.CoverImage = newCoverImage;

			// Assert

			Assert.AreSame(newCoverImage, target.CoverImage);
			Assert.AreEqual(2, target.Images.Count);
		}

		[Test]
		public void CoverImageSetter_IfDiscContainsNoCoverImage_AddsNewCoverImage()
		{
			// Arrange

			var newCoverImage = new DiscImage { ImageType = DiscImageType.Cover };

			var target = new Disc
			{
				Images = { new DiscImage { ImageType = DiscImageType.None } },
			};

			// Act

			target.CoverImage = newCoverImage;

			// Assert

			Assert.AreSame(newCoverImage, target.CoverImage);
			Assert.AreEqual(2, target.Images.Count);
		}

		[Test]
		public void CoverImageSetter_IfNewImageIsNotCoverImage_ThrowsInvalidOperationException()
		{
			// Arrange

			var newCoverImage = new DiscImage { ImageType = DiscImageType.None };

			var target = new Disc();

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.CoverImage = newCoverImage);
		}

		[Test]
		public void CoverImageSetter_IfValueIsNull_RemovesExistingDiscCoverImage()
		{
			// Arrange

			var target = new Disc
			{
				Images =
				{
					new DiscImage { ImageType = DiscImageType.Cover },
					new DiscImage { ImageType = DiscImageType.None }
				},
			};

			// Act

			target.CoverImage = null;

			// Assert

			Assert.IsNull(target.CoverImage);
			Assert.AreEqual(1, target.Images.Count);
		}

		[Test]
		public void IsDeletedGetter_WhenSomeSongsAreNotDeleted_ReturnsFalse()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { DeleteDate = null },
					new Song { DeleteDate = new DateTime(2017, 09, 30) },
				}
			};

			// Act & Assert

			Assert.IsFalse(disc.IsDeleted);
		}

		[Test]
		public void IsDeletedGetter_WhenAllSongsAreDeleted_ReturnsTrue()
		{
			// Arrange

			var disc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { DeleteDate = new DateTime(2017, 09, 30) },
					new Song { DeleteDate = new DateTime(2017, 09, 30) },
				}
			};

			// Act & Assert

			Assert.IsTrue(disc.IsDeleted);
		}
	}
}
