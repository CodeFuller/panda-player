using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class ArtistDiscViewItemTests
	{
		[Test]
		public void Constructor_ForArtistDiscWhenMajorSongsDoNotHaveArtist_SetsDiscArtistForAllSongs()
		{
			// Arrange

			AddedSongInfo[] songs =
			{
				new AddedSongInfo("SomeSourcePath")
				{
					Artist = "Lappi",
					FullTitle = "Lappi - I. Eramaajarvi"
				},

				new AddedSongInfo("SomeSourcePath")
				{
					Artist = null,
				},

				new AddedSongInfo("SomeSourcePath")
				{
					Artist = null,
				},
			};

			var discInfo = new AddedDiscInfo(songs)
			{
				DiscTitle = "Some Title",
				DiscType = DsicType.ArtistDisc,
				Artist = "Nightwish",
			};

			// Act

			var disc = new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), null);

			// Assert

			var addedSong = disc.Songs.First();
			Assert.AreEqual("Nightwish", addedSong.Song.Artist.Name);
			Assert.AreEqual("Lappi - I. Eramaajarvi", addedSong.Song.Title);
		}

		[Test]
		public void Constructor_WhenMajorSongsHaveArtist_LeavesArtistForSuchSongs()
		{
			// Arrange

			AddedSongInfo[] songs =
			{
				new AddedSongInfo("SomeSourcePath")
				{
					Artist = "Nirvana",
					FullTitle = "Nirvana - Nevermind"
				},

				new AddedSongInfo("SomeSourcePath")
				{
					Artist = "Metallica",
					FullTitle = "Metallica - Unforgiven"
				},

				new AddedSongInfo("SomeSourcePath")
				{
					Artist = null,
				},
			};

			var discInfo = new AddedDiscInfo(songs)
			{
				DiscTitle = "Some Title",
				DiscType = DsicType.ArtistDisc,
				Artist = "AC/DC",
			};

			var artists = new[]
			{
				new Artist { Name = "AC/DC" },
				new Artist { Name = "Metallica" },
				new Artist { Name = "Nirvana" },
			};

			// Act

			var disc = new ArtistDiscViewItem(discInfo, artists, Enumerable.Empty<Genre>(), null);

			// Assert

			var addedSongs = disc.Songs.ToList();
			Assert.AreEqual("Nirvana", addedSongs[0].Song.Artist.Name);
			Assert.AreEqual("Metallica", addedSongs[1].Song.Artist.Name);
			Assert.AreEqual("AC/DC", addedSongs[2].Song.Artist.Name);
		}

		[Test]
		public void Constructor_IfDiscArtistIsSet_LookupsArtistInListOfAvailableArtists()
		{
			// Arrange

			var artist1 = new Artist { Name = "Some Artist 1" };
			var artist2 = new Artist { Name = "Some Artist 2" };
			var artist3 = new Artist { Name = "Some Artist 3" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist 2",
			};

			// Act

			var target = new ArtistDiscViewItem(discInfo, new[] { artist1, artist2, artist3 }, Enumerable.Empty<Genre>(), null);

			// Assert

			Assert.AreSame(artist2, target.Artist);
		}

		[Test]
		public void Constructor_IfDiscArtistNotFoundInAvailableArtists_ThrowsInvalidOperationException()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), null));
		}

		[Test]
		public void Constructor_IfDiscArtistIsNotSet_ThrowsInvalidOperationException()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = null,
			};

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), null));
		}

		[Test]
		public void ArtistSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var artist = new Artist { Name = "Some Artist" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(discInfo, new[] { artist }, Enumerable.Empty<Genre>(), null);

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Artist = artist);
		}

		[Test]
		public void ArtistIsEditableGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), null);

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
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), null);

			// Act & Assert

			Assert.IsFalse(target.ArtistIsNotFilled);
		}

		[Test]
		public void YearSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), null);

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Year = 2017);
		}

		[Test]
		public void DiscTypeTitleGetter_ReturnsCorrectDiscTypeTitle()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				DiscTitle = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), null);

			// Act & Assert

			Assert.AreEqual("Artist Disc", target.DiscTypeTitle);
		}
	}
}
