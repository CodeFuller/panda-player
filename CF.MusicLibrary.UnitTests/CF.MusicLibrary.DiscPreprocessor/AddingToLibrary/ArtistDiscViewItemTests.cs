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
				DiscType = DsicType.ArtistDisc,
				Artist = "Nightwish",
			};

			//	Act

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo,
				new[] { new Artist { Name = "Nightwish" } }, Enumerable.Empty<Uri>(),
				Arg.Any<Uri>(), Enumerable.Empty<Genre>(), Arg.Any<Genre>());

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
				Enumerable.Empty<Uri>(),
				Arg.Any<Uri>(), Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			var addedSongs = disc.Songs.ToList();
			Assert.AreEqual("Nirvana", addedSongs[0].Song.Artist.Name);
			Assert.AreEqual("Metallica", addedSongs[1].Song.Artist.Name);
			Assert.AreEqual("AC/DC", addedSongs[2].Song.Artist.Name);
		}

		[Test]
		public void RequiredDataIsFilled_WhenGenreIsNotFilled_ReturnsFalse()
		{
			var discInfo = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				DiscType = DsicType.ArtistDisc,
				Artist = "AC/DC",
			};

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo,
				new[] { new Artist { Name = "AC/DC" } }, Enumerable.Empty<Uri>(),
				new Uri("SomeUri", UriKind.Relative), Enumerable.Empty<Genre>(), null);

			Assert.IsFalse(disc.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenDestinationUriIsNotFilled_ReturnsFalse()
		{
			var discInfo = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				DiscType = DsicType.ArtistDisc,
				Artist = "AC/DC",
			};

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo,
				new[] { new Artist { Name = "AC/DC" } }, Enumerable.Empty<Uri>(),
				null, Enumerable.Empty<Genre>(), new Genre());

			Assert.IsFalse(disc.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenAllRequiredFieldsAreNotFilled_ReturnsTrue()
		{
			var discInfo = new AddedDiscInfo(new AddedSongInfo[] {})
			{
				DiscType = DsicType.ArtistDisc,
				Artist = "AC/DC",
			};

			var disc = new ArtistDiscViewItem(Arg.Any<string>(), discInfo,
				new[] { new Artist { Name = "AC/DC" } }, Enumerable.Empty<Uri>(),
				new Uri("SomeUri", UriKind.Relative), Enumerable.Empty<Genre>(), new Genre());

			Assert.IsTrue(disc.RequiredDataIsFilled);
		}
	}
}
