using System;
using MusicLibrary.Core.Objects.Images;
using NUnit.Framework;

namespace MusicLibrary.Library.Tests
{
	[TestFixture]
	public class LibraryStructurerTests
	{
		[Test]
		public void ReplaceDiscPartInImageUri_ReturnsCorrectUri()
		{
			// Arrange

			var target = new LibraryStructurer();

			// Act

			var imageUri = target.ReplaceDiscPartInImageUri(new Uri("/SomeDir/SubDir/NewDisc", UriKind.Relative), new Uri("/SomeDir/OldDisc/SomeImage.img", UriKind.Relative));

			// Assert

			Assert.AreEqual(new Uri("/SomeDir/SubDir/NewDisc/SomeImage.img", UriKind.Relative), imageUri);
		}

		[Test]
		public void GetDiscCoverImageUri_ReturnsCorrectUri()
		{
			// Arrange

			var imageInfo = new ImageInfo
			{
				Format = ImageFormatType.Jpeg,
			};

			var target = new LibraryStructurer();

			// Act

			var imageUri = target.GetDiscCoverImageUri(new Uri("/SomeDir/SomeDisc", UriKind.Relative), imageInfo);

			// Assert

			Assert.AreEqual(new Uri("/SomeDir/SomeDisc/cover.jpg", UriKind.Relative), imageUri);
		}
	}
}
