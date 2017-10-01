using System;
using System.Collections.Generic;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Common.DiscArt;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Common.DiscArt
{
	[TestFixture]
	public class DiscArtImageFileTests
	{
		[Test]
		public void Constructor_IfDiscArtValidatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtImageFile(null, Substitute.For<IFileSystemFacade>()));
		}

		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), null));
		}

		[Test]
		public void Load_IfFileNameArgumentIsNull_ThrowsArgumentNullException()
		{
			//	Arrange

			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>());

			//	Act & Assert

			Assert.Throws<ArgumentNullException>(() => target.Load(null, false));
		}

		[Test]
		public void Load_IfPreviousFileWasSetToTemporaryFile_DeletesPreviousFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), fileSystemMock);

			target.Load("PreviousImageFile.jpg", true);

			//	Act

			target.Load("NewImageFile.jpg", true);

			//	Assert

			fileSystemMock.Received(1).DeleteFile("PreviousImageFile.jpg");
		}

		[Test]
		public void Load_IfPreviousFileWasSetToNonTemporaryFile_DoesNotDeletePreviousFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), fileSystemMock);

			target.Load("PreviousImageFile.jpg", false);

			//	Act

			target.Load("NewImageFile.jpg", true);

			//	Assert

			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void Load_SetsPropertiesCorrectly()
		{
			//	Arrange

			DiscArtImageInfo imageInfo = new DiscArtImageInfo();
			IDiscArtValidator discArtValidatorStub = Substitute.For<IDiscArtValidator>();
			discArtValidatorStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);

			var target = new DiscArtImageFile(discArtValidatorStub, Substitute.For<IFileSystemFacade>());

			//	Act

			target.Load("DiscImageFile.jpg", true);

			//	Assert

			Assert.AreSame(imageInfo, target.ImageInfo);
			Assert.AreSame("DiscImageFile.jpg", target.ImageFileName);
			Assert.AreEqual(true, target.IsTemporaryFile);
		}

		[Test]
		public void Load_RaisesPropertyChangedEventForAffectedProperties()
		{
			//	Arrange

			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>());

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			//	Act

			target.Load("DiscImageFile.jpg", true);

			//	Assert

			CollectionAssert.Contains(changedProperties, nameof(DiscArtImageFile.ImageFileName));
			CollectionAssert.Contains(changedProperties, nameof(DiscArtImageFile.ImageIsValid));
			CollectionAssert.Contains(changedProperties, nameof(DiscArtImageFile.ImageProperties));
			CollectionAssert.Contains(changedProperties, nameof(DiscArtImageFile.ImageStatus));
		}

		[Test]
		public void Unload_IfPreviousFileWasSetToTemporaryFile_DeletesPreviousFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), fileSystemMock);

			target.Load("PreviousImageFile.jpg", true);

			//	Act

			target.Unload();

			//	Assert

			fileSystemMock.Received(1).DeleteFile("PreviousImageFile.jpg");
		}

		[Test]
		public void Unload_IfPreviousFileWasSetToNonTemporaryFile_DoesNotDeletePreviousFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), fileSystemMock);

			target.Load("PreviousImageFile.jpg", false);

			//	Act

			target.Unload();

			//	Assert

			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void ImageIsValidGetter_IfImageIsNotSet_ReturnsFalse()
		{
			//	Arrange

			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>());

			//	Act & Assert

			Assert.IsFalse(target.ImageIsValid);
		}

		[Test]
		public void ImageIsValidGetter_IfCurrentImageIsValid_ReturnsTrue()
		{
			//	Arrange

			DiscArtImageInfo imageInfo = new DiscArtImageInfo();
			IDiscArtValidator discArtValidatorStub = Substitute.For<IDiscArtValidator>();
			discArtValidatorStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);
			discArtValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(DiscArtValidationResults.ImageIsOk);

			var target = new DiscArtImageFile(discArtValidatorStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			//	Act & Assert

			Assert.IsTrue(target.ImageIsValid);
		}

		[Test]
		public void ImageIsValidGetter_IfCurrentImageIsNotValid_ReturnsTrue()
		{
			//	Arrange

			DiscArtImageInfo imageInfo = new DiscArtImageInfo();
			IDiscArtValidator discArtValidatorStub = Substitute.For<IDiscArtValidator>();
			discArtValidatorStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);
			discArtValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(DiscArtValidationResults.ImageIsTooSmall);

			var target = new DiscArtImageFile(discArtValidatorStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			//	Act & Assert

			Assert.IsFalse(target.ImageIsValid);
		}

		[Test]
		public void ImagePropertiesGetter_IfImageIsNotSet_ReturnsNotNull()
		{
			//	Arrange

			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>());

			//	Act & Assert

			Assert.IsNotNull(target.ImageProperties);
		}

		[Test]
		public void ImageStatusGetter_IfImageIsNotSet_ReturnsNotNull()
		{
			//	Arrange

			var target = new DiscArtImageFile(Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>());

			//	Act & Assert

			Assert.IsNotNull(target.ImageStatus);
		}

		[Test]
		public void ImageStatusGetter_ReturnsSemicolonSeparatedListOfValidationHints()
		{
			//	Arrange

			DiscArtImageInfo imageInfo = new DiscArtImageInfo();
			IDiscArtValidator discArtValidatorStub = Substitute.For<IDiscArtValidator>();
			discArtValidatorStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);
			discArtValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(DiscArtValidationResults.ImageIsTooSmall | DiscArtValidationResults.UnsupportedFormat);
			discArtValidatorStub.GetValidationResultsHints(DiscArtValidationResults.ImageIsTooSmall | DiscArtValidationResults.UnsupportedFormat).Returns(new []{ "Hint1", "Hint2" });

			var target = new DiscArtImageFile(discArtValidatorStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			//	Act & Assert

			Assert.AreEqual("Hint1; Hint2", target.ImageStatus);
		}
	}
}
