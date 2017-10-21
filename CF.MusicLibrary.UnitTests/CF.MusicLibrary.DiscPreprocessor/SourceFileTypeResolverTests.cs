using System;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.DiscPreprocessor;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor
{
	[TestFixture]
	public class SourceFileTypeResolverTests
	{
		[Test]
		public void Constructor_IfDiscImageValidatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SourceFileTypeResolver(null));
		}

		[Test]
		public void GetSourceFileType_IfFileHasMP3Extension_ReturnsSourceFileTypeSong()
		{
			//	Arrange

			var target = new SourceFileTypeResolver(Substitute.For<IDiscImageValidator>());

			//	Act

			SourceFileType fileType = target.GetSourceFileType("SomeFilePath.mp3");

			//	Assert

			Assert.AreEqual(SourceFileType.Song, fileType);
		}

		[Test]
		public void GetSourceFileType_IfFileHasSupportedDiscImageFormat_ReturnsSourceFileTypeImage()
		{
			//	Arrange

			IDiscImageValidator discImageValidatorStub = Substitute.For<IDiscImageValidator>();
			discImageValidatorStub.IsSupportedFileFormat("SomeFilePath").Returns(true);

			var target = new SourceFileTypeResolver(discImageValidatorStub);

			//	Act

			SourceFileType fileType = target.GetSourceFileType("SomeFilePath");

			//	Assert

			Assert.AreEqual(SourceFileType.Image, fileType);
		}
	}
}
