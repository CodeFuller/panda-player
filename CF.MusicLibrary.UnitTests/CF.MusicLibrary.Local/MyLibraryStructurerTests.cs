using System;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.Local;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Local
{
	[TestFixture]
	public class MyLibraryStructurerTests
	{
		[Test]
		public void ReplaceDiscPartInImageUri_ReturnsCorrectUri()
		{
			//	Arrange

			var target = new MyLibraryStructurer();

			//	Act

			var imageUri = target.ReplaceDiscPartInImageUri(new Uri("/SomeDir/SubDir/NewDisc", UriKind.Relative), new Uri("/SomeDir/OldDisc/SomeImage.img", UriKind.Relative));

			//	Assert

			Assert.AreEqual(new Uri("/SomeDir/SubDir/NewDisc/SomeImage.img", UriKind.Relative), imageUri);
		}

		[Test]
		public void GetDiscCoverImageUri_ReturnsCorrectUri()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Format = ImageFormatType.Jpeg,
			};

			var target = new MyLibraryStructurer();

			//	Act

			var imageUri = target.GetDiscCoverImageUri(new Uri("/SomeDir/SomeDisc", UriKind.Relative), imageInfo);

			//	Assert

			Assert.AreEqual(new Uri("/SomeDir/SomeDisc/cover.jpg", UriKind.Relative), imageUri);
		}
	}
}
