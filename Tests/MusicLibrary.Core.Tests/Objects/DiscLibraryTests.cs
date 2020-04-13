using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using NUnit.Framework;

namespace MusicLibrary.Core.Tests.Objects
{
	[TestFixture]
	public class DiscLibraryTests
	{
		[Test]
		public void Load_IfLoaderWasNotset_Throws()
		{
			// Arrange

			var target = new DiscLibrary();

			// Act & Assert

			Assert.Throws<AggregateException>(() => target.Load().Wait());
		}

		[Test]
		public void Load_FillsDiscsFromProvidedDiscLoader()
		{
			// Arrange

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song() } },
				new Disc { SongsUnordered = new[] { new Song() } },
			};
			var target = new DiscLibrary(() => Task.FromResult(discs.AsEnumerable()));

			// Act

			target.Load().Wait();

			// Assert

			CollectionAssert.AreEqual(discs, target.Discs);
		}

		[Test]
		public void Load_FillsPlaybacksPassedForDiscsCorrectly()
		{
			// Arrange

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song { LastPlaybackTime = new DateTime(2017, 09, 28) } } },
				new Disc { SongsUnordered = new[] { new Song { LastPlaybackTime = new DateTime(2017, 09, 30) } } },
				new Disc { SongsUnordered = new[] { new Song { LastPlaybackTime = null } } },
				new Disc { SongsUnordered = new[] { new Song { LastPlaybackTime = new DateTime(2017, 09, 29) } } },
			};
			var target = new DiscLibrary(() => Task.FromResult(discs.AsEnumerable()));

			// Act

			target.Load().Wait();

			// Assert

			var loadedDiscs = target.Discs.ToList();
			Assert.AreEqual(2, loadedDiscs[0].PlaybacksPassed);
			Assert.AreEqual(0, loadedDiscs[1].PlaybacksPassed);
			Assert.AreEqual(Int32.MaxValue, loadedDiscs[2].PlaybacksPassed);
			Assert.AreEqual(1, loadedDiscs[3].PlaybacksPassed);
		}

		[Test]
		public void DiscsGetter_ReturnsAllActiveDiscs()
		{
			// Arrange

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song { DeleteDate = null } } },
				new Disc { SongsUnordered = new[] { new Song { DeleteDate = null } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedDiscs = target.Discs;

			// Assert

			CollectionAssert.AreEqual(discs, returnedDiscs);
		}

		[Test]
		public void DiscsGetter_DoesNotReturnDeletedDiscs()
		{
			// Arrange

			var target = new DiscLibrary(new[] { new Disc { SongsUnordered = new[] { new Song { DeleteDate = new DateTime(2017, 09, 30) } } } });

			// Act & Assert

			CollectionAssert.IsEmpty(target.Discs);
		}

		[Test]
		public void DiscsGetter_IfLibraryWasNotLoaded_ThrowsInvalidOperationException()
		{
			// Arrange

			var target = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));

			// Act & Assert

			IEnumerable<Disc> discs;
			Assert.Throws<InvalidOperationException>(() => discs = target.Discs);

			// Avoiding uncovered lambda code (Task.FromResult(Enumerable.Empty<Disc>()))
			target.Load().Wait();
		}

		[Test]
		public void AllDiscsGetter_ReturnsAllDiscs()
		{
			// Arrange

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song { DeleteDate = null } } },
				new Disc { SongsUnordered = new[] { new Song { DeleteDate = null } } },
				new Disc { SongsUnordered = new[] { new Song { DeleteDate = new DateTime(2017, 09, 30) } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedDiscs = target.AllDiscs;

			// Assert

			CollectionAssert.AreEqual(discs, returnedDiscs);
		}

		[Test]
		public void SongsGetter_ReturnsAllActiveSongs()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var song3 = new Song();

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { song1, song2 } },
				new Disc { SongsUnordered = new[] { song3 } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedSongs = target.Songs;

			// Assert

			CollectionAssert.AreEqual(new[] { song1, song2, song3 }, returnedSongs);
		}

		[Test]
		public void SongsGetter_DoesNotReturnDeletedSongs()
		{
			// Arrange

			var target = new DiscLibrary(new[] { new Disc { SongsUnordered = new[] { new Song { DeleteDate = new DateTime(2017, 09, 30) } } } });

			// Act

			var returnedSongs = target.Songs;

			// Assert

			CollectionAssert.IsEmpty(returnedSongs);
		}

		[Test]
		public void AllSongsGetter_ReturnsAllSongs()
		{
			// Arrange

			var song1 = new Song { DeleteDate = null };
			var song2 = new Song { DeleteDate = new DateTime(2017, 09, 30) };
			var song3 = new Song { DeleteDate = null };

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { song1, song2 } },
				new Disc { SongsUnordered = new[] { song3 } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedSongs = target.AllSongs;

			// Assert

			CollectionAssert.AreEqual(new[] { song1, song2, song3 }, returnedSongs);
		}

		[Test]
		public void ArtistsGetter_ReturnsArtistsForAllActiveSongs()
		{
			// Arrange

			Artist artist1 = new Artist();
			Artist artist2 = new Artist();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[] { new Song { Artist = artist1 }, new Song { Artist = artist2 }, },
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.Artists;

			// Assert

			CollectionAssert.AreEqual(new[] { artist1, artist2 }, returnedArtists);
		}

		[Test]
		public void ArtistsGetter_DoesNotReturnArtistsForDeletedSongs()
		{
			// Arrange

			Artist artist1 = new Artist();
			Artist artist2 = new Artist();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Artist = artist1,
							DeleteDate = new DateTime(2017, 09, 30),
						},
						new Song { Artist = artist2 },
					},
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.Artists;

			// Assert

			CollectionAssert.AreEqual(new[] { artist2 }, returnedArtists);
		}

		[Test]
		public void ArtistsGetter_DoesNotReturnDuplicatedArtists()
		{
			// Arrange

			Artist artist = new Artist();

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song { Artist = artist } } },
				new Disc { SongsUnordered = new[] { new Song { Artist = artist } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.Artists;

			// Assert

			CollectionAssert.AreEqual(new[] { artist }, returnedArtists);
		}

		[Test]
		public void ArtistsGetter_DoesNotReturnNullArtist()
		{
			// Arrange

			var discs = new[]
			{
				new Disc { SongsUnordered = new[] { new Song { Artist = null } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.Artists;

			// Assert

			CollectionAssert.IsEmpty(returnedArtists);
		}

		[Test]
		public void AllArtistsGetter_ReturnsAllArtists()
		{
			// Arrange

			Artist artist1 = new Artist();
			Artist artist2 = new Artist();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
						{
							new Song
							{
								Artist = artist1,
								DeleteDate = new DateTime(2017, 09, 30),
							},
							new Song { Artist = artist2 },
						},
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.AllArtists;

			// Assert

			CollectionAssert.AreEqual(new[] { artist1, artist2 }, returnedArtists);
		}

		[Test]
		public void AllArtistsGetter_DoesNotReturnDuplicatedArtists()
		{
			// Arrange

			Artist artist = new Artist();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Artist = artist,
							DeleteDate = new DateTime(2017, 09, 30),
						},
					},
				},
				new Disc { SongsUnordered = new[] { new Song { Artist = artist } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.AllArtists;

			// Assert

			CollectionAssert.AreEqual(new[] { artist }, returnedArtists);
		}

		[Test]
		public void AllArtistsGetter_DoesNotReturnNullArtist()
		{
			// Arrange

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Artist = null,
							DeleteDate = new DateTime(2017, 09, 30),
						},
					},
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedArtists = target.AllArtists;

			// Assert

			CollectionAssert.IsEmpty(returnedArtists);
		}

		[Test]
		public void GenresGetter_ReturnsAllGenres()
		{
			// Arrange

			Genre genre1 = new Genre();
			Genre genre2 = new Genre();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Genre = genre1,
							DeleteDate = new DateTime(2017, 09, 30),
						},
						new Song { Genre = genre2 },
					},
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedGenres = target.Genres;

			// Assert

			CollectionAssert.AreEqual(new[] { genre1, genre2 }, returnedGenres);
		}

		[Test]
		public void GenresGetter_DoesNotReturnDuplicatedGenres()
		{
			// Arrange

			Genre genre = new Genre();

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Genre = genre,
							DeleteDate = new DateTime(2017, 09, 30),
						},
					},
				},
				new Disc { SongsUnordered = new[] { new Song { Genre = genre } } },
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedGenres = target.Genres;

			// Assert

			CollectionAssert.AreEqual(new[] { genre }, returnedGenres);
		}

		[Test]
		public void GenresGetter_DoesNotReturnNullGenre()
		{
			// Arrange

			var discs = new[]
			{
				new Disc
				{
					SongsUnordered = new[]
					{
						new Song
						{
							Genre = null,
							DeleteDate = new DateTime(2017, 09, 30),
						},
					},
				},
			};

			var target = new DiscLibrary(discs);

			// Act

			var returnedGenres = target.Genres;

			// Assert

			CollectionAssert.IsEmpty(returnedGenres);
		}
	}
}
