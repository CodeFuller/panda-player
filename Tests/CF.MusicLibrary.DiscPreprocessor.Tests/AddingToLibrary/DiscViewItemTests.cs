using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Attributes;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class DiscViewItemTests
	{
		[ExcludeFromTestCoverage("Empty stub of base abstract class")]
		private class ConcreteDiscViewItem : DiscViewItem
		{
			public ConcreteDiscViewItem(AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
				: base(disc, availableArtists, availableGenres)
			{
				Disc = new Disc
				{
					Uri = disc.UriWithinStorage,
					Title = disc.Title,
				};
			}

			public override string DiscTypeTitle { get; }
			public override Artist Artist { get; set; }
			public override bool ArtistIsEditable { get; }
			public override bool ArtistIsNotFilled { get; }
			public override string AlbumTitle { get; set; }
			public override bool AlbumTitleIsEditable { get; }
			public override bool YearIsEditable { get; }
			public override bool RequiredDataIsFilled { get; }

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
		public void Constructor_InitializesSourcePathCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				SourcePath = "Some Source Path",
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual("Some Source Path", target.SourcePath);
		}

		[Test]
		public void Constructor_InitializesAvailableArtistsCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var artists = new[]
			{
				new Artist(),
				new Artist(),
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, artists, Enumerable.Empty<Genre>());

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

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), genres);

			//	Assert

			CollectionAssert.AreEqual(genres, target.AvailableGenres);
		}

		[Test]
		public void Constructor_InitializesDestinationUriCorrectly()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				UriWithinStorage = new Uri("/Some/Disc/Uri", UriKind.Relative),
			};

			//	Act

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Assert

			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), target.DestinationUri);
		}

		[Test]
		public void WarnAboutDiscTypeGetter_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Artist = new Artist { Id = 0 };

			//	Act & Assert

			Assert.IsFalse(target.WarnAboutDiscType);
		}

		[Test]
		public void ArtistIsNewGetter_ForNewArtist_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Artist = new Artist { Id = 0 };

			//	Act & Assert

			Assert.IsTrue(target.ArtistIsNew);
		}

		[Test]
		public void ArtistIsNewGetter_ForExistingArtist_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Artist = new Artist { Id = 1 };

			//	Act & Assert

			Assert.IsFalse(target.ArtistIsNew);
		}

		[Test]
		public void TitleGetter_ReturnsTitleOfInnerDisc()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				Title = "Some Disc Title",
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.AreEqual("Some Disc Title", target.DiscTitle);
		}

		[TestCase(2017)]
		[TestCase(null)]
		public void YearGetter_ReturnsCorrectYearValue(short? year)
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());
			target.Year = year;

			//	Act & Assert

			Assert.AreEqual(year, target.Year);
		}

		[Test]
		public void GenreIsNotFilledGetter_WhenGenreIsSet_ReturnsFalse()
		{
			//	Arrange

			Genre genre = new Genre();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), new[] { genre });
			target.Genre = genre;

			//	Act & Assert

			Assert.IsFalse(target.GenreIsNotFilled);
		}

		[Test]
		public void GenreIsNotFilledGetter_WhenGenreIsNotSet_ReturnsTrue()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), new[] { new Genre() });

			//	Act & Assert

			Assert.IsTrue(target.GenreIsNotFilled);
		}

		[Test]
		public void DestinationUri_ReturnsUriOfInnerDisc()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				UriWithinStorage = new Uri("/SomeDiscUri", UriKind.Relative),
			};

			var target = new ConcreteDiscViewItem(discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			//	Act & Assert

			Assert.AreEqual(new Uri("/SomeDiscUri", UriKind.Relative), target.DestinationUri);
		}

		[Test]
		public void LookupArtist_IfArtistPresentsInAvailableArtists_ReturnsCorrectArtist()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var artist1 = new Artist { Name = "Artist 1" };
			var artist2 = new Artist { Name = "Artist 2" };
			var artists = new[] { artist1, artist2 };

			var target = new ConcreteDiscViewItem(discInfo, artists, new[] { new Genre() });

			//	Act

			var returnedArtist = target.CallLookupArtist("Artist 2");

			//	Act & Assert

			Assert.AreSame(artist2, returnedArtist);
		}

		[Test]
		public void LookupArtist_IfArtistDoesNotPresentsnAvailableArtists_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());

			var target = new ConcreteDiscViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, new[] { new Genre() });

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.CallLookupArtist("Another Artist"));
		}
	}
}
