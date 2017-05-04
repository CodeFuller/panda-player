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
	public class AddedCompilationAlbumWithoutArtistInfoTests
	{
		[Test]
		public void RequiredDataIsFilled_WhenGenreIsNotFilled_ReturnsFalse()
		{
			var album = new AddedCompilationAlbumWithoutArtistInfo(Arg.Any<string>(), new AlbumInfo(Enumerable.Empty<SongInfo>()),
				Arg.Any<Uri>(), Enumerable.Empty<Genre>());

			Assert.IsFalse(album.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenGenreIsFilled_ReturnsTrue()
		{
			var album = new AddedCompilationAlbumWithoutArtistInfo(Arg.Any<string>(), new AlbumInfo(Enumerable.Empty<SongInfo>()),
				Arg.Any<Uri>(), Enumerable.Empty<Genre>())
			{
				Genre = new Genre()
			};

			Assert.IsTrue(album.RequiredDataIsFilled);
		}
	}
}
