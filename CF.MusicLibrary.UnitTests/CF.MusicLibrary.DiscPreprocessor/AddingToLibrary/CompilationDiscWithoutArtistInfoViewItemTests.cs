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
	public class CompilationDiscWithoutArtistInfoViewItemTests
	{
		[Test]
		public void RequiredDataIsFilled_WhenGenreIsNotFilled_ReturnsFalse()
		{
			var disc = new CompilationDiscWithoutArtistInfoViewItem(Arg.Any<string>(), new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>()),
				Enumerable.Empty<Artist>(), Arg.Any<Uri>(), Enumerable.Empty<Genre>());

			Assert.IsFalse(disc.RequiredDataIsFilled);
		}

		[Test]
		public void RequiredDataIsFilled_WhenGenreIsFilled_ReturnsTrue()
		{
			var disc = new CompilationDiscWithoutArtistInfoViewItem(Arg.Any<string>(), new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>()),
				Enumerable.Empty<Artist>(), Arg.Any<Uri>(), Enumerable.Empty<Genre>())
			{
				Genre = new Genre()
			};

			Assert.IsTrue(disc.RequiredDataIsFilled);
		}
	}
}
