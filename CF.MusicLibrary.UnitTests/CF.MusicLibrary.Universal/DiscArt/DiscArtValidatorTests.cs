using System;
using System.Drawing.Imaging;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Universal.DiscArt;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Universal.DiscArt
{
	[TestFixture]
	public class DiscArtValidatorTests
	{
		[Test]
		public void Constructor_WhenFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DiscArtValidator(null));
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageIsValid_ReturnsImageIsOk()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.AreEqual(DiscArtValidationResults.ImageIsOk, validationResults);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageWidthIsTooSmall_SetsImageIsTooSmallValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 50,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooSmall) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHeightIsTooSmall_SetsImageIsTooSmallValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 500,
				Height = 50,
				FileSize = 500 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooSmall) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageWidthIsTooBig_SetsImageIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 5000,
				Height = 500,
				FileSize = 500 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHeightIsTooBig_SetsImageIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 500,
				Height = 5000,
				FileSize = 500 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageFileSizeIsTooBig_SetsFileSizeIsTooBigValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormat.Jpeg,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.FileSizeIsTooBig) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfImageHasUnsupportedFormat_SetsUnsupportedFormatValue()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 500,
				Height = 500,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormat.Png,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.UnsupportedFormat) != 0);
		}

		[Test]
		public void ValidateDiscCoverImage_IfMultipleValidationsFail_SetsAllValidationResults()
		{
			//	Arrange

			var imageInfo = new DiscArtImageInfo
			{
				Width = 50,
				Height = 5000,
				FileSize = 500 * 1024 * 1024,
				Format = ImageFormat.Png,
			};

			var target = new DiscArtValidator(Substitute.For<IFileSystemFacade>());

			//	Act

			var validationResults = target.ValidateDiscCoverImage(imageInfo);

			//	Assert

			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooSmall) != 0);
			Assert.IsTrue((validationResults & DiscArtValidationResults.ImageIsTooBig) != 0);
			Assert.IsTrue((validationResults & DiscArtValidationResults.FileSizeIsTooBig) != 0);
			Assert.IsTrue((validationResults & DiscArtValidationResults.UnsupportedFormat) != 0);
		}
	}
}
