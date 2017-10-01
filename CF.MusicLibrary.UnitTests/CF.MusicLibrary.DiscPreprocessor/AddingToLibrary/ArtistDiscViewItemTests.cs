using System;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class ArtistDiscViewItemTests
	{
		[Test]
		public void Constructor_ForArtistDiscWhenMajorSongsDoNotHaveArtist_SetsDiscArtistForAllSongs()
		{
			//	Arrange

			AddedSongInfo[] songs =
			{
				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = "Lappi",
					FullTitle = "Lappi - I. Eramaajarvi"
				},

				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = null,
				},

				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = null,
				},
			};

			var discInfo = new AddedDiscInfo(songs)
			{
				Title = "Some Title",
				DiscType = DsicType.ArtistDisc,
				Artist = "Nightwish",
			};

			//	Act

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			var addedSong = disc.Songs.First();
			Assert.AreEqual("Nightwish", addedSong.Song.Artist.Name);
			Assert.AreEqual("Lappi - I. Eramaajarvi", addedSong.Song.Title);
		}

		[Test]
		public void Constructor_WhenMajorSongsHaveArtist_LeavesArtistForSuchSongs()
		{
			//	Arrange

			AddedSongInfo[] songs =
			{
				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = "Nirvana",
					FullTitle = "Nirvana - Nevermind"
				},

				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = "Metallica",
					FullTitle = "Metallica - Unforgiven"
				},

				new AddedSongInfo(Arg.Any<string>())
				{
					Artist = null,
				},
			};

			var discInfo = new AddedDiscInfo(songs)
			{
				Title = "Some Title",
				DiscType = DsicType.ArtistDisc,
				Artist = "AC/DC",
			};

			//	Act

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo,
				new[]
				{
					new Artist { Name = "AC/DC" },
					new Artist { Name = "Metallica" },
					new Artist { Name = "Nirvana" },
				},
				Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			var addedSongs = disc.Songs.ToList();
			Assert.AreEqual("Nirvana", addedSongs[0].Song.Artist.Name);
			Assert.AreEqual("Metallica", addedSongs[1].Song.Artist.Name);
			Assert.AreEqual("AC/DC", addedSongs[2].Song.Artist.Name);
		}

		[Test]
		public void Constructor_IfDiscArtistIsSet_LookupsArtistInListOfAvailableArtists()
		{
			//	Arrange

			var artist1 = new Artist { Name = "Some Artist 1" };
			var artist2 = new Artist { Name = "Some Artist 2" };
			var artist3 = new Artist { Name = "Some Artist 3" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist 2",
			};

			//	Act

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { artist1, artist2, artist3 }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			Assert.AreSame(artist2, target.Artist);
		}

		[Test]
		public void Constructor_IfDiscArtistNotFoundInAvailableArtists_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() =>new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>()));
		}

		[Test]
		public void Constructor_IfDiscArtistIsNotSet_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = null,
			};

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>()));
		}

		[Test]
		public void ArtistSetter_ThrowsInvalidOperationException()
		{
			//	Arrange

			var artist = new Artist { Name = "Some Artist" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { artist }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Artist = artist);
		}

		[Test]
		public void ArtistIsEditableGetter_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Act & Assert

			Assert.IsFalse(target.ArtistIsEditable);
		}

		[Test]
		public void ArtistIsNotFilledGetter_ReturnsFalse()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Act & Assert

			Assert.IsFalse(target.ArtistIsNotFilled);
		}

		[Test]
		public void YearSetter_ThrowsInvalidOperationException()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Year = 2017);
		}

		[Test]
		public void DiscTypeTitleGetter_ReturnsCorrectDiscTypeTitle()
		{
			//	Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.ArtistDisc,
				Title = "Some Title",
				Artist = "Some Artist",
			};

			var target = new ArtistDiscViewItem(Arg.Any<string>(), discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Act & Assert

			Assert.AreEqual("Artist Disc", target.DiscTypeTitle);
		}
	}
}
