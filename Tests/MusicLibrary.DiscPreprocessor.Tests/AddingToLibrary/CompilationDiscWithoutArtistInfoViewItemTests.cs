using System.Linq;
using MusicLibrary.Core.Objects;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;
using MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class CompilationDiscWithoutArtistInfoViewItemTests
	{
		[Test]
		public void ArtistIsEditableGetter_ReturnsTrue()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithoutArtistInfoViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsTrue(target.ArtistIsEditable);
		}

		[Test]
		public void ArtistIsNotFilledGetter_IfArtistIsNotSet_ReturnsTrue()
		{
			// Arrange

			var artist = new Artist { Name = "Some Artist" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithoutArtistInfoViewItem(discInfo, new[] { artist }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsTrue(target.ArtistIsNotFilled);
		}

		[Test]
		public void ArtistIsNotFilledGetter_IfArtistIsSet_ReturnsFalse()
		{
			// Arrange

			var artist = new Artist { Name = "Some Artist" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithoutArtistInfoViewItem(discInfo, new[] { artist }, Enumerable.Empty<Genre>());
			target.Artist = artist;

			// Act & Assert

			Assert.IsFalse(target.ArtistIsNotFilled);
		}

		[Test]
		public void DiscTypeTitleGetter_ReturnsCorrectDiscTypeTitle()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithoutArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithoutArtistInfoViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual("Compilation without Artists", target.DiscTypeTitle);
		}
	}
}
