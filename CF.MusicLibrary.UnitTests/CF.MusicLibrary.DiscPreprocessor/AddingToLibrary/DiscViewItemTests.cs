﻿using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Attributes;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class DiscViewItemTests
	{
		private class ConcreteDiscViewItem : DiscViewItem
		{
			public ConcreteDiscViewItem(string sourcePath, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
				: base(sourcePath, disc, availableArtists, availableGenres)
			{
			}

			[ExcludeFromTestCoverage("Unused stub of base abstract method")]
			public override string DiscTypeTitle { get; }

			public override Artist Artist { get; set; }

			[ExcludeFromTestCoverage("Unused stub of base abstract method")]
			public override bool ArtistIsEditable { get; }

			[ExcludeFromTestCoverage("Unused stub of base abstract method")]
			public override bool ArtistIsNotFilled { get; }

			protected override Artist GetSongArtist(AddedSongInfo song)
			{
				return null;
			}

			public Artist CallLookupArtist(string artistName)
			{
				return LookupArtist(artistName);
			}
		}

		[Test]
		public void Constructor_IfDiscTitleIsCorrect_InitializesDiscTitleCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Title", target.AlbumTitle);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void Constructor_IfDiscTitleIsEmpty_ThrowsInvalidOperationException(string discTitle)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = discTitle,
			};

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>()));
		}

		[Test]
		public void Constructor_InitializesSourcePathCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Source", target.SourcePath);
		}

		[TestCase("Broken Crown Halo (CD 1)", "Broken Crown Halo")]
		[TestCase("Broken Crown Halo", "Broken Crown Halo")]
		public void Constructor_InitializesAlbumTitleCorrectly(string discTitle, string expectedAlbumTitle)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = discTitle,
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual(expectedAlbumTitle, target.AlbumTitle);
		}

		[Test]
		public void Constructor_InitializesAvailableArtistsCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var artists = new[]
			{
				new Artist(),
				new Artist(),
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, artists, Enumerable.Empty<Genre>());

			//	Assert

			CollectionAssert.AreEqual(artists, target.AvailableArtists);
		}

		[Test]
		public void Constructor_InitializesAvailableGenresCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var genres = new[]
			{
				new Genre(),
				new Genre(),
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), genres);

			//	Assert

			CollectionAssert.AreEqual(genres, target.AvailableGenres);
		}

		[Test]
		public void Constructor_InitializesDestinationUriCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			//	Act

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), target.DestinationUri);
		}

		[Test]
		public void ArtistIsNewGetter_ForNewArtist_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Artist = new Artist { Id = 0 };

			//	Act & Assert

			Assert.IsTrue(target.ArtistIsNew);
		}

		[Test]
		public void ArtistIsNewGetter_ForExistingArtist_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Artist = new Artist { Id = 1 };

			//	Act & Assert

			Assert.IsFalse(target.ArtistIsNew);
		}

		[Test]
		public void AlbumTitleMatchesDiscTitleGetter_WhenAlbumTitleEqualsDiscTitle_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.AlbumTitle = "Some Title";

			//	Act & Assert

			Assert.IsTrue(target.AlbumTitleMatchesDiscTitle);
		}

		[Test]
		public void AlbumTitleMatchesDiscTitleGetter_WhenAlbumTitleDoesNotEqualDiscTitle_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.AlbumTitle = "Album Title";

			//	Act & Assert

			Assert.IsFalse(target.AlbumTitleMatchesDiscTitle);
		}

		[TestCase(2017)]
		[TestCase(null)]
		public void YearGetter_ReturnsCorrectYearValue(short? year)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Year = year;

			//	Act & Assert

			Assert.AreEqual(year, target.Year);
		}

		[Test]
		public void YearIsNotFilledGetter_IfYearIsSet_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			target.Year = 2017;

			//	Act & Assert

			Assert.IsFalse(target.YearIsNotFilled);
		}

		[Test]
		public void YearIsNotFilledGetter_IfYearIsNotSet_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem("Some Source", discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.IsTrue(target.YearIsNotFilled);
		}

		[Test]
		public void GenreIsNotFilledGetter_WhenGenreIsSet_ReturnsFalse()
		{
			//	Arrange

			Genre genre = new Genre();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), new[] { genre });
			target.Genre = genre;

			//	Act & Assert

			Assert.IsFalse(target.GenreIsNotFilled);
		}

		[Test]
		public void GenreIsNotFilledGetter_WhenGenreIsNotSet_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			//	Act & Assert

			Assert.IsTrue(target.GenreIsNotFilled);
		}

		[Test]
		public void RequiredDataIsFilledGetter_WhenAllRequiredDataIsSet_ReturnsTrue()
		{
			//	Arrange

			Genre genre = new Genre();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), new[] { genre });
			target.Genre = genre;

			//	Act & Assert

			Assert.IsTrue(target.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilledGetter_WhenGenreIsNotSet_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			//	Act & Assert

			Assert.IsFalse(target.RequiredDataIsFilled);
		}

		[Test]
		public void SongsGetter_FillsSongsDataCorrectly()
		{
			//	Arrange

			var genre = new Genre();

			AddedSongInfo addedSongInfo = new AddedSongInfo("SongSourcePath")
			{
				Artist = "Song Artist",
				Track = 1,
				Title = "Song Title",
			};

			var discInfo = new AddedDiscInfo(new[] {addedSongInfo})
			{
				Title = "Disc Title (CD 1)",
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			target.Genre = genre;
			target.Year = 2017;

			//	Act

			var songs = target.Songs.ToList();

			//	Assert

			Assert.AreEqual(1, songs.Count);
			var songInfo = songs.Single();
			var song = songInfo.Song;
			var songDisc = song.Disc;
			Assert.AreEqual("Disc Title (CD 1)", songDisc.Title);
			Assert.AreEqual("Disc Title", songDisc.AlbumTitle);
			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), songDisc.Uri);
			Assert.AreEqual(1, song.TrackNumber);
			Assert.AreEqual(2017, song.Year);
			Assert.AreEqual("Song Title", song.Title);
			Assert.AreSame(genre, song.Genre);
			Assert.IsNull(song.Rating);
			Assert.IsNull(song.LastPlaybackTime);
			Assert.AreEqual(0, song.PlaybacksCount);
			Assert.AreEqual("SongSourcePath", songInfo.SourceFileName);
		}

		[Test]
		public void LookupArtist_IfArtistPresentsInAvailableArtists_ReturnsCorrectArtist()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var artist1 = new Artist { Name = "Artist 1" };
			var artist2 = new Artist { Name = "Artist 2" };
			var artists = new[] { artist1, artist2 };

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, artists, new[] { new Genre() });

			//	Act

			var returnedArtist = target.CallLookupArtist("Artist 2");

			//	Act & Assert

			Assert.AreSame(artist2, returnedArtist);
		}

		[Test]
		public void LookupArtist_IfArtistDoesNotPresentsnAvailableArtists_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Title",
			};

			var target = new ConcreteDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.CallLookupArtist("Another Artist"));
		}
	}
}
