using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Objects.Images;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Common.Images
{
	[TestFixture]
	public class DiscImageValidatorTests
	{
		[Test]
		public void ValidateDiscCoverImage_IfImageIsValid_ReturnsImageIsOk()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.AreEqual(ImageValidationResults.ImageIsOk, validationResults);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageWidthIsTooSmall_SetsImageIsTooSmallValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 50,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooSmall) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHeightIsTooSmall_SetsImageIsTooSmallValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 500,
				Height = 50,
				FileSize = 500 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooSmall) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageWidthIsTooBig_SetsImageIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 10000,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHeightIsTooBig_SetsImageIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 500,
				Height = 10000,
				FileSize = 500 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageFileSizeIsTooBig_SetsFileSizeIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormatType.Jpeg,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.FileSizeIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHasUnsupportedFormat_SetsUnsupportedFormatValue()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormatType.Unsupported,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.UnsupportedFormat) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfMultipleValidationsFail_SetsAllValidationResults()
		{
			//	Arrange

			var imageInfo = new ImageInfo
			{
				Width = 50,
				Height = 10000,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormatType.Unsupported,
			};

			var target = new DiscImageValidator();

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooSmall) != 0);
			Assert.IsTrue((validationResults & ImageValidationResults.ImageIsTooBig) != 0);
			Assert.IsTrue((validationResults & ImageValidationResults.FileSizeIsTooBig) != 0);
			Assert.IsTrue((validationResults & ImageValidationResults.UnsupportedFormat) != 0);
		}

		[Test]
		public void IsSupportedFileFormat_ForFileNameWithExtensionOfSupportedFormat_ReturnsTrue()
		{
			//	Arrange

			var target = new DiscImageValidator();

			//	Act

			bool isSupportedFormat = target.IsSupportedFileFormat("SomeFile.jpg");

			//	Assert

			Assert.IsTrue(isSupportedFormat);
		}

		[Test]
		public void IsSupportedFileFormat_ForFileNameWithExtensionOfUnsupportedFormat_ReturnsFalse()
		{
			//	Arrange

			var target = new DiscImageValidator();

			//	Act

			bool isSupportedFormat = target.IsSupportedFileFormat("SomeFile.ico");

			//	Assert

			Assert.IsFalse(isSupportedFormat);
		}
	}
}
