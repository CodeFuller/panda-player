using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class ExistingDiscViewItemTests
	{
		[Test]
		public void DiscTypeTitleGetter_ReturnsCorrectDiscTypeTitle()
		{
			// Arrange

			var existingDisc = new Disc();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual("Existing Disc", target.DiscTypeTitle);
		}

		[Test]
		public void WarnAboutDiscTypeGetter_ReturnsTrue()
		{
			// Arrange

			var existingDisc = new Disc();

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsTrue(target.WarnAboutDiscType);
		}

		[Test]
		public void DiscTitleGetter_ReturnsTitleOfExistingDisc()
		{
			// Arrange

			var existingDisc = new Disc
			{
				Title = "Existing Title",
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "New Title",
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual("Existing Title", target.DiscTitle);
		}

		[Test]
		public void AlbumTitleGetter_ReturnsAlbumTitleOfExistingDisc()
		{
			// Arrange

			var existingDisc = new Disc
			{
				AlbumTitle = "Existing Album Title",
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "New Title",
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual("Existing Album Title", target.AlbumTitle);
		}

		[Test]
		public void AlbumTitleSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var existingDisc = new Disc
			{
				AlbumTitle = "Existing Album Title",
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "New Title",
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.AlbumTitle = "Very New Title");
		}

		[Test]
		public void AlbumTitleIsEditableGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());
			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.AlbumTitleIsEditable);
		}

		[Test]
		public void WarnAboutUnequalAlbumTitleGetter_ReturnsFalse()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Disc Title",
				AlbumTitle = "Album Title",
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());
			var target = new ExistingDiscViewItem(disc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Sanity check
			Assert.AreNotEqual(target.DiscTitle, target.AlbumTitle);

			// Act & Assert

			Assert.IsFalse(target.WarnAboutUnequalAlbumTitle);
		}

		[Test]
		public void YearIsEditableGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());
			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.YearIsEditable);
		}

		[Test]
		public void WarnAboutNotFilledYearGetter_ReturnsFalse()
		{
			// Arrange

			var disc = new Disc
			{
				Title = "Disc Title",
				AlbumTitle = "Album Title",
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>());
			var target = new ExistingDiscViewItem(disc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Sanity check
			Assert.IsNull(target.Year);

			// Act & Assert

			Assert.IsFalse(target.WarnAboutNotFilledYear);
		}

		[Test]
		public void ArtistGetter_IfExistingDiscHasArtist_ReturnsThisArtist()
		{
			// Arrange

			var existingArtist = new Artist();
			var existingDisc = new Disc
			{
				SongsUnordered = new[] { new Song { Artist = existingArtist } },
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreSame(existingArtist, target.Artist);
		}

		[Test]
		public void ArtistGetter_IfExistingDiscDoesNotHaveArtist_ReturnsNull()
		{
			// Arrange

			var existingDisc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { Artist = new Artist() },
					new Song { Artist = new Artist() },
				},
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsNull(target.Artist);
		}

		[Test]
		public void ArtistSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Artist = new Artist());
		}

		[Test]
		public void ArtistIsEditableGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.ArtistIsEditable);
		}

		[Test]
		public void ArtistIsNotFilledGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.ArtistIsNotFilled);
		}

		[Test]
		public void YearGetter_IfExistingDiscHasYear_ReturnsThisYear()
		{
			// Arrange

			var existingDisc = new Disc
			{
				SongsUnordered = new[] { new Song { Year = 2017 } },
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual(2017, target.Year);
		}

		[Test]
		public void YearGetter_IfExistingDiscDoesNotHaveYear_ReturnsNull()
		{
			// Arrange

			var existingDisc = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { Year = 2016 },
					new Song { Year = 2017 },
				},
			};

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsNull(target.Year);
		}

		[Test]
		public void YearSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Year = 2017);
		}

		[Test]
		public void RequiredDataIsFilledGetter_ReturnsTrue()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(new Disc(), discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsTrue(target.RequiredDataIsFilled);
		}

		[Test]
		public void Songs_IfParsedSongArtistIsNull_FillsArtistFromExistingDisc()
		{
			// Arrange

			var existingArtist = new Artist();
			var existingDisc = new Disc
			{
				SongsUnordered = new[] { new Song { Artist = existingArtist } },
			};

			var discInfo = new AddedDiscInfo(new[]
				{
					new AddedSongInfo("SourcePath") { Artist = null }
				})
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, Enumerable.Empty<Artist>(), Enumerable.Empty<Genre>());

			// Act

			var addedSongs = target.Songs.ToList();

			// Assert

			Assert.AreSame(existingArtist, addedSongs.Single().Song.Artist);
		}

		[Test]
		public void Songs_IfParsedSongArtistIsNotNull_LookupsArtistInArtistsList()
		{
			// Arrange

			var existingArtist = new Artist();
			var someNewArtist = new Artist { Name = "Some New Artist" };
			var existingDisc = new Disc
			{
				SongsUnordered = new[] { new Song { Artist = existingArtist } },
			};

			var discInfo = new AddedDiscInfo(new[]
				{
					new AddedSongInfo("SourcePath") { Artist = "Some New Artist" }
				})
			{
				DiscType = DsicType.ArtistDisc,
			};

			var target = new ExistingDiscViewItem(existingDisc, discInfo, new[] { someNewArtist }, Enumerable.Empty<Genre>());

			// Act

			var addedSongs = target.Songs.ToList();

			// Assert

			Assert.AreSame(someNewArtist, addedSongs.Single().Song.Artist);
		}
	}
}
