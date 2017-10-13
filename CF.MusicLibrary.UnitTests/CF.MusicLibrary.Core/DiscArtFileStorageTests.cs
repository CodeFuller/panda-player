using System;
using System.Drawing.Imaging;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Common.DiscArt;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Core
{
	[TestFixture]
	public class DiscArtFileStorageTests
	{
		[Test]
		public void Constructor_IfImageFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtFileStorage(null, Substitute.For<IFileSystemFacade>()));
		}

		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtFileStorage(Substitute.For<IImageFacade>(), null));
		}

		[Test]
		public void GetDiscCoverImageFileName_IfDiscDirectoryContainsOneCoverImageOfSupportedFormat_ReturnsPathToThisFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemFacade = Substitute.For<IFileSystemFacade>();
			fileSystemFacade.FileExists(@"RootDir\SomeDisc\cover.jpg").Returns(false);
			fileSystemFacade.FileExists(@"RootDir\SomeDisc\cover.png").Returns(true);
			var target = new DiscArtFileStorage(Substitute.For<IImageFacade>(), fileSystemFacade);

			//	Act

			var imageFileName = target.GetDiscCoverImageFileName(@"RootDir\SomeDisc");

			//	Assert

			Assert.AreEqual(@"RootDir\SomeDisc\cover.png", imageFileName);
		}

		[Test]
		public void GetDiscCoverImageFileName_IfDiscDirectoryContainsNoCoverImageFiles_ReturnsNull()
		{
			//	Arrange

			var target = new DiscArtFileStorage(Substitute.For<IImageFacade>(), Substitute.For<IFileSystemFacade>());

			//	Act

			var imageFileName = target.GetDiscCoverImageFileName(@"RootDir\SomeDisc");

			//	Assert

			Assert.IsNull(imageFileName);
		}

		[Test]
		public void GetDiscCoverImageFileName_IfDiscDirectoryContainsMultipleCoverImageOfSupportedFormat_ReturnsPathToThisFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemFacade = Substitute.For<IFileSystemFacade>();
			fileSystemFacade.FileExists(Arg.Any<string>()).Returns(true);
			var target = new DiscArtFileStorage(Substitute.For<IImageFacade>(), fileSystemFacade);

			//	Act & Assert

			string imageFileName;
			Assert.Throws<InvalidOperationException>(() => imageFileName = target.GetDiscCoverImageFileName(@"RootDir\SomeDisc"));
		}

		[Test]
		public void StoreDiscCoverImage_IfSourceImageHasUnsupportedFormat_ThrowsInvalidOperationExceptionAndDoesNotDeleteExistingCoverImageFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(Arg.Any<string>()).Returns(true);

			IImageFacade imageFacade = Substitute.For<IImageFacade>();
			imageFacade.FromFile("SomeNewCover.img").Returns(imageFacade);
			imageFacade.RawFormat.Returns(ImageFormat.Icon);
			var target = new DiscArtFileStorage(imageFacade, Substitute.For<IFileSystemFacade>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.img"));
			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void StoreDiscCoverImage_IfPreviousDiscCoverImageExists_DeletesPreviousDiscCoverImage()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDisc\cover.jpg").Returns(true);

			IImageFacade imageFacade = Substitute.For<IImageFacade>();
			imageFacade.FromFile("SomeNewCover.img").Returns(imageFacade);
			imageFacade.RawFormat.Returns(ImageFormat.Jpeg);
			var target = new DiscArtFileStorage(imageFacade, fileSystemMock);

			//	Act

			target.StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.img");

			//	Assert

			Received.InOrder(() => {
				fileSystemMock.Received(1).ClearReadOnlyAttribute(@"RootDir\SomeDisc\cover.jpg");
				fileSystemMock.Received(1).DeleteFile(@"RootDir\SomeDisc\cover.jpg");
			});
		}

		[Test]
		public void StoreDiscCoverImage_IfPreviousDiscCoverDoesNotExist_DoesNotAttemptToDeletesPreviousDiscCoverImage()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDisc\cover.jpg").Returns(false);

			IImageFacade imageFacade = Substitute.For<IImageFacade>();
			imageFacade.FromFile("SomeNewCover.img").Returns(imageFacade);
			imageFacade.RawFormat.Returns(ImageFormat.Jpeg);
			var target = new DiscArtFileStorage(imageFacade, fileSystemMock);

			//	Act

			target.StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.img");

			//	Assert

			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void StoreDiscCoverImage_IfSourceImageHasSupportedFormat_StoresCoverImageAtCorrespondingFileName()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();

			IImageFacade imageFacade = Substitute.For<IImageFacade>();
			imageFacade.FromFile("SomeNewCover.img").Returns(imageFacade);
			imageFacade.RawFormat.Returns(ImageFormat.Png);

			var target = new DiscArtFileStorage(imageFacade, fileSystemMock);

			//	Act

			target.StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.img");

			//	Assert

			fileSystemMock.Received(1).CopyFile("SomeNewCover.img", @"RootDir\SomeDisc\cover.png");
		}

		[Test]
		public void StoreDiscCoverImage_SetsReadOnlyAttributeForDestinationCoverImageFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();

			IImageFacade imageFacade = Substitute.For<IImageFacade>();
			imageFacade.FromFile("SomeNewCover.img").Returns(imageFacade);
			imageFacade.RawFormat.Returns(ImageFormat.Png);

			var target = new DiscArtFileStorage(imageFacade, fileSystemMock);

			//	Act

			target.StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.img");

			//	Assert

			fileSystemMock.Received(1).SetReadOnlyAttribute(@"RootDir\SomeDisc\cover.png");
		}

		[Test]
		public void IsCoverImageFile_IfFileHasOneOfPredefinedCoverImageFilenames_ReturnsTrue()
		{
			//	Arrange

			var target = new DiscArtFileStorage(Substitute.For<IImageFacade>(), Substitute.For<IFileSystemFacade>());

			//	Act

			var isCoverImage = target.IsCoverImageFile(@"RootDir\SomeDisc\cover.png");

			//	Assert

			Assert.IsTrue(isCoverImage);
		}

		[Test]
		public void IsCoverImageFile_IfFileHasNoneOfPredefinedCoverImageFilenames_ReturnsFalse()
		{
			//	Arrange

			var target = new DiscArtFileStorage(Substitute.For<IImageFacade>(), Substitute.For<IFileSystemFacade>());

			//	Act

			var isCoverImage = target.IsCoverImageFile(@"RootDir\SomeDisc\Picture.jpg");

			//	Assert

			Assert.IsFalse(isCoverImage);
		}
	}
}
