using System.Collections.Generic;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Objects.Images;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.Common.Tests.Images
{
	[TestFixture]
	public class ImageFileTests
	{
		[Test]
		public void Load_IfPreviousFileWasSetToTemporaryFile_DeletesPreviousFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo(Arg.Any<string>()).Returns(x => new ImageInfo { FileName = (string)x[0] });
			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), imageInfoProviderStub, fileSystemMock);

			target.Load("PreviousImageFile.jpg", true);

			// Act

			target.Load("NewImageFile.jpg", true);

			// Assert

			fileSystemMock.Received(1).DeleteFile("PreviousImageFile.jpg");
		}

		[Test]
		public void Load_IfPreviousFileWasSetToNonTemporaryFile_DoesNotDeletePreviousFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), Substitute.For<IImageInfoProvider>(), fileSystemMock);

			target.Load("PreviousImageFile.jpg", false);

			// Act

			target.Load("NewImageFile.jpg", true);

			// Assert

			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void Load_SetsPropertiesCorrectly()
		{
			// Arrange

			ImageInfo imageInfo = new ImageInfo
			{
				FileName = "DiscImageFile.jpg",
			};
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);

			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), imageInfoProviderStub, Substitute.For<IFileSystemFacade>());

			// Act

			target.Load("DiscImageFile.jpg", true);

			// Assert

			Assert.AreSame(imageInfo, target.ImageInfo);
			Assert.AreSame("DiscImageFile.jpg", target.ImageFileName);
			Assert.AreEqual(true, target.IsTemporaryFile);
		}

		[Test]
		public void Load_RaisesPropertyChangedEventForAffectedProperties()
		{
			// Arrange

			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo("DiscImageFile.jpg").Returns(x => new ImageInfo { FileName = "DiscImageFile.jpg" });
			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), imageInfoProviderStub, Substitute.For<IFileSystemFacade>());

			var changedProperties = new List<string>();
			target.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

			// Act

			target.Load("DiscImageFile.jpg", true);

			// Assert

			Assert.Contains(nameof(ImageFile.ImageInfo), changedProperties);
			Assert.Contains(nameof(ImageFile.ImageFileName), changedProperties);
			Assert.Contains(nameof(ImageFile.ImageIsValid), changedProperties);
			Assert.Contains(nameof(ImageFile.ImageProperties), changedProperties);
			Assert.Contains(nameof(ImageFile.ImageStatus), changedProperties);
		}

		[Test]
		public void Unload_IfPreviousFileWasSetToTemporaryFile_DeletesPreviousFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo(Arg.Any<string>()).Returns(x => new ImageInfo { FileName = (string)x[0] });
			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), imageInfoProviderStub, fileSystemMock);

			target.Load("PreviousImageFile.jpg", true);

			// Act

			target.Unload();

			// Assert

			fileSystemMock.Received(1).DeleteFile("PreviousImageFile.jpg");
		}

		[Test]
		public void Unload_IfPreviousFileWasSetToNonTemporaryFile_DoesNotDeletePreviousFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo(Arg.Any<string>()).Returns(x => new ImageInfo { FileName = (string)x[0] });
			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), imageInfoProviderStub, fileSystemMock);

			target.Load("PreviousImageFile.jpg", false);

			// Act

			target.Unload();

			// Assert

			fileSystemMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void ImageIsValidGetter_IfImageIsNotSet_ReturnsFalse()
		{
			// Arrange

			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), Substitute.For<IImageInfoProvider>(), Substitute.For<IFileSystemFacade>());

			// Act & Assert

			Assert.IsFalse(target.ImageIsValid);
		}

		[Test]
		public void ImageIsValidGetter_IfCurrentImageIsValid_ReturnsTrue()
		{
			// Arrange

			ImageInfo imageInfo = new ImageInfo();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);

			IDiscImageValidator discImageValidatorStub = Substitute.For<IDiscImageValidator>();
			discImageValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(ImageValidationResults.ImageIsOk);

			var target = new ImageFile(discImageValidatorStub, imageInfoProviderStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			// Act & Assert

			Assert.IsTrue(target.ImageIsValid);
		}

		[Test]
		public void ImageIsValidGetter_IfCurrentImageIsNotValid_ReturnsTrue()
		{
			// Arrange

			ImageInfo imageInfo = new ImageInfo();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);

			IDiscImageValidator discImageValidatorStub = Substitute.For<IDiscImageValidator>();
			discImageValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(ImageValidationResults.ImageIsTooSmall);

			var target = new ImageFile(discImageValidatorStub, imageInfoProviderStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			// Act & Assert

			Assert.IsFalse(target.ImageIsValid);
		}

		[Test]
		public void ImagePropertiesGetter_IfImageIsNotSet_ReturnsNotNull()
		{
			// Arrange

			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), Substitute.For<IImageInfoProvider>(), Substitute.For<IFileSystemFacade>());

			// Act & Assert

			Assert.IsNotNull(target.ImageProperties);
		}

		[Test]
		public void ImageStatusGetter_IfImageIsNotSet_ReturnsNotNull()
		{
			// Arrange

			var target = new ImageFile(Substitute.For<IDiscImageValidator>(), Substitute.For<IImageInfoProvider>(), Substitute.For<IFileSystemFacade>());

			// Act & Assert

			Assert.IsNotNull(target.ImageStatus);
		}

		[Test]
		public void ImageStatusGetter_ReturnsSemicolonSeparatedListOfValidationHints()
		{
			// Arrange

			ImageInfo imageInfo = new ImageInfo();
			IImageInfoProvider imageInfoProviderStub = Substitute.For<IImageInfoProvider>();
			imageInfoProviderStub.GetImageInfo("DiscImageFile.jpg").Returns(imageInfo);

			IDiscImageValidator discImageValidatorStub = Substitute.For<IDiscImageValidator>();
			discImageValidatorStub.ValidateDiscCoverImage(imageInfo).Returns(ImageValidationResults.ImageIsTooSmall | ImageValidationResults.UnsupportedFormat);
			discImageValidatorStub.GetValidationResultsHints(ImageValidationResults.ImageIsTooSmall | ImageValidationResults.UnsupportedFormat)
				.Returns(new[] { "Hint1", "Hint2" });

			var target = new ImageFile(discImageValidatorStub, imageInfoProviderStub, Substitute.For<IFileSystemFacade>());
			target.Load("DiscImageFile.jpg", true);

			// Act & Assert

			Assert.AreEqual("Hint1; Hint2", target.ImageStatus);
		}
	}
}
