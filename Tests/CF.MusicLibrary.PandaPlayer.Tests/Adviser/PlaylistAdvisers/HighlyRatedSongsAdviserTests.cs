using System;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.Tests;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.PandaPlayer.Tests.Adviser.PlaylistAdvisers
{
	[TestFixture]
	public class HighlyRatedSongsAdviserTests
	{
		[Test]
		public void Advise_ReturnsSongsWithHighRatingListenedEarlierThanConfiguredTerm()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = new DateTime(2017, 09, 01),
			}, 12).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs.ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 07));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs, advises.Single().Songs);
		}

		[Test]
		public void Advise_ReturnsHighlyRatedSongsWithoutPlaybacks()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 12).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs.ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs, advises.Single().Songs);
		}

		[Test]
		public void Advise_DoesNotReturnHighlyRatedSongsListenedRecently()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var song = new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = new DateTime(2017, 09, 05),
			};

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = Enumerable.Repeat(song, 12).ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary);

			// Assert

			CollectionAssert.IsEmpty(advises);
		}

		[Test]
		public void Advise_DoesNotReturnNotHighlyRatedSongs()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs = Enumerable.Repeat(new Song
			{
				Rating = Rating.R9,
				LastPlaybackTime = null,
			}, 12);

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs.ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 06));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary);

			// Assert

			CollectionAssert.IsEmpty(advises);
		}

		[Test]
		public void Advise_IfTooMuchSongsAreAdvised_SplitsThemIntoSmallerPlaylists()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs1 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 12).ToList();

			var songs2 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 12).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs1.Concat(songs2).ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs1, advises[0].Songs);
			CollectionAssert.AreEqual(songs2, advises[1].Songs);
		}

		[Test]
		public void Advise_IfNumberOfAdvisedSongsIsNotEnough_ReturnsNoPlaylists()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 5).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs.ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			CollectionAssert.IsEmpty(advises);
		}

		[Test]
		public void Advise_IfNumberOfSongsInLastAdviseIsNotEnough_SkipsLastAdvise()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs1 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 12).ToList();

			var songs2 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = null,
			}, 11).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs1.Concat(songs2).ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(Substitute.For<IAdviseFactorsProvider>(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(songs1, advises.Single().Songs);
		}

		[Test]
		public void Advise_IfSongsAreAdvised_ReturnsSongsWithHigherRatingFirst()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					},

					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R9,
						Days = 60,
					},
				}
			};

			var songs1 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R9,
				LastPlaybackTime = new DateTime(2017, 01, 01),
			}, 12).ToList();

			var songs2 = Enumerable.Repeat(new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = new DateTime(2017, 01, 01),
			}, 12).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs1.Concat(songs2).ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(new AdviseFactorsProvider(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs2, advises[0].Songs);
			CollectionAssert.AreEqual(songs1, advises[1].Songs);
		}

		[Test]
		public void Advise_IfSongsAreAdvised_ReturnsSongsWithGreaterPlaybackAgeFirst()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					}
				}
			};

			var songs1 = Enumerable.Range(1, 12).Select(i => new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = new DateTime(2017, 02, 1 + i),
			}).ToList();

			var songs2 = Enumerable.Range(1, 12).Select(i => new Song
			{
				Rating = Rating.R10,
				LastPlaybackTime = new DateTime(2017, 01, 1 + i),
			}).ToList();

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = songs1.Concat(songs2).ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(new AdviseFactorsProvider(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(2, advises.Count);
			CollectionAssert.AreEqual(songs2, advises[0].Songs);
			CollectionAssert.AreEqual(songs1, advises[1].Songs);
		}

		[Test]
		public void Advise_IfSongsAreAdvised_OrdersThemByProductOfRatingAndPlaybackAgeFactors()
		{
			// Arrange

			var settings = new HighlyRatedSongsAdviserSettings
			{
				OneAdviseSongsNumber = 12,
				MaxUnlistenedTerms =
				{
					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R10,
						Days = 30,
					},

					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R9,
						Days = 60,
					},

					new MaxUnlistenedSongTerm
					{
						Rating = Rating.R8,
						Days = 90,
					},
				}
			};

			// Rank = 2 * 0 = 0
			var song01 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 12) };

			// Rank = 2 * 1 = 2
			var song02 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 11) };

			// Rank = 2 * 2 = 4
			var song03 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 10) };

			// Rank = 2 * 3 = 6
			var song04 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 09) };

			// Rank = 2 * 4 = 8
			var song05 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 08) };

			// Rank = 2 * 5 = 10
			var song06 = new Song { Rating = Rating.R10, LastPlaybackTime = new DateTime(2017, 01, 07) };

			// Rank = 1 * 6 = 6
			var song07 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 06) };

			// Rank = 1 * 7 = 7
			var song08 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 05) };

			// Rank = 1 * 8 = 8
			var song09 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 04) };

			// Rank = 1 * 9 = 9
			var song10 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 03) };

			// Rank = 1 * 10 = 10
			var song11 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 02) };

			// Rank = 1 * 11 = 11
			var song12 = new Song { Rating = Rating.R9, LastPlaybackTime = new DateTime(2017, 01, 01) };

			var discLibrary = new DiscLibrary(new[] { new Disc { SongsUnordered = new[] { song01, song02, song03, song04, song05, song06, song07, song08, song09, song10, song11, song12 }.ToCollection() } });

			var dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 01));

			var target = new HighlyRatedSongsAdviser(new AdviseFactorsProvider(), dateTimeStub, settings.StubOptions());

			// Act

			var advises = target.Advise(discLibrary).ToList();

			// Assert

			Assert.AreEqual(1, advises.Count);
			CollectionAssert.AreEqual(new[] { song12, song06, song11, song10, song05, song09, song08, song04, song07, song03, song02, song01 }, advises.Single().Songs);
		}
	}
}
