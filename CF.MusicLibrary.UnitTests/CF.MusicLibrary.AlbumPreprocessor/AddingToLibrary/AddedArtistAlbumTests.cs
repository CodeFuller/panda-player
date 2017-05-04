using System;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	[TestFixture]
	public class AddedArtistAlbumTests
	{
		[Test]
		public void Constructor_WhenMajorSongsDoNotHaveArtist_ClearsArtistForAllSongs()
		{
			//	Arrange

			SongInfo[] songs =
			{
				new SongInfo(Arg.Any<string>())
				{
					Artist = "Lappi",
					FullTitle = "Lappi - I. Eramaajarvi"
				},

				new SongInfo(Arg.Any<string>())
				{
					Artist = null,
				},

				new SongInfo(Arg.Any<string>())
				{
					Artist = null,
				},
			};

			var albumInfo = new AlbumInfo(songs);

			//	Act

			var album = new AddedArtistAlbum(Arg.Any<string>(), albumInfo,
				Enumerable.Empty<Uri>(), Arg.Any<Uri>(),
				Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			var song = album.Songs.First();
			Assert.IsNull(song.Artist);
			Assert.AreEqual("Lappi - I. Eramaajarvi", song.Title);
		}

		[Test]
		public void Constructor_WhenMajorSongsHaveArtist_LeavesArtistForSuchSongs()
		{
			//	Arrange

			SongInfo[] songs =
			{
				new SongInfo(Arg.Any<string>())
				{
					Artist = "Nirvana",
					FullTitle = "Nirvana - Nevermind"
				},

				new SongInfo(Arg.Any<string>())
				{
					Artist = "Metallica",
					FullTitle = "Metallica - Unforgiven"
				},

				new SongInfo(Arg.Any<string>())
				{
					Artist = null,
				},
			};

			var albumInfo = new AlbumInfo(songs);

			//	Act

			var album = new AddedArtistAlbum(Arg.Any<string>(), albumInfo,
				Enumerable.Empty<Uri>(), Arg.Any<Uri>(),
				Enumerable.Empty<Genre>(), Arg.Any<Genre>());

			//	Assert

			var albumSongs = album.Songs.ToList();
			Assert.AreEqual("Nirvana", albumSongs[0].Artist);
			Assert.AreEqual("Metallica", albumSongs[1].Artist);
			Assert.IsNull(albumSongs[2].Artist);
		}

		[Test]
		public void RequiredDataIsFilled_WhenGenreIsNotFilled_ReturnsFalse()
		{
			var album = new AddedArtistAlbum(Arg.Any<string>(), new AlbumInfo(Enumerable.Empty<SongInfo>()), 
				Enumerable.Empty<Uri>(), new Uri("SomeUri", UriKind.Relative), 
				Enumerable.Empty<Genre>(), null);

			Assert.IsFalse(album.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenDestinationUriIsNotFilled_ReturnsFalse()
		{
			var album = new AddedArtistAlbum(Arg.Any<string>(), new AlbumInfo(Enumerable.Empty<SongInfo>()),
				Enumerable.Empty<Uri>(), null,
				Enumerable.Empty<Genre>(), new Genre());

			Assert.IsFalse(album.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenAllRequiredFieldsAreNotFilled_ReturnsTrue()
		{
			var album = new AddedArtistAlbum(Arg.Any<string>(), new AlbumInfo(Enumerable.Empty<SongInfo>()),
				Enumerable.Empty<Uri>(), new Uri("SomeUri", UriKind.Relative),
				Enumerable.Empty<Genre>(), new Genre());

			Assert.IsTrue(album.RequiredDataIsFilled);
		}
	}
}
