using System;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class DiscImageViewItemTests
	{
		[Test]
		public void Constructor_FillsPropertiesCorrectly()
		{
			// Arrange

			var disc = new Disc();

			IImageFile imageFileStub = Substitute.For<IImageFile>();

			// Act

			var target = new DiscImageViewItem(disc, DiscImageType.Cover, imageFileStub);

			// Assert

			Assert.AreSame(disc, target.Disc);
			Assert.AreEqual(DiscImageType.Cover, target.ImageType);
		}

		[Test]
		public void DiscUriGetter_ReturnsUriOfInnerDisc()
		{
			// Arrange

			var disc = new Disc
			{
				Uri = new Uri("/Some/Disc/Uri", UriKind.Relative)
			};

			var target = new DiscImageViewItem(disc, DiscImageType.Cover, Substitute.For<IImageFile>());

			// Act & Assert

			Assert.AreEqual(new Uri("/Some/Disc/Uri", UriKind.Relative), target.DiscUri);
		}

		[Test]
		public void ImageIsValidGetter_IfImageFileIsValid_ReturnsTrue()
		{
			// Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(true);

			var target = new DiscImageViewItem(new Disc(), DiscImageType.Cover, imageFileStub);

			// Act & Assert

			Assert.IsTrue(target.ImageIsValid);
		}

		[Test]
		public void ImageIsValidGetter_IfImageFileIsNotValid_ReturnsFalse()
		{
			// Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(false);

			var target = new DiscImageViewItem(new Disc(), DiscImageType.Cover, imageFileStub);

			// Act & Assert

			Assert.IsFalse(target.ImageIsValid);
		}

		[Test]
		public void ImageStatusGetter_IfImageIsValid_ReturnsPropertiesOfInnerImageFile()
		{
			// Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(true);
			imageFileStub.ImageProperties.Returns("Some Properties");

			var target = new DiscImageViewItem(new Disc(), DiscImageType.Cover, imageFileStub);

			// Act & Assert

			Assert.AreEqual("Some Properties", target.ImageStatus);
		}

		[Test]
		public void ImageStatusGetter_IfImageIsNotValid_ReturnsStatusOfInnerImageFile()
		{
			// Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(false);
			imageFileStub.ImageStatus.Returns("Some Status");

			var target = new DiscImageViewItem(new Disc(), DiscImageType.Cover, imageFileStub);

			// Act & Assert

			Assert.AreEqual("Some Status", target.ImageStatus);
		}

		[Test]
		public void ImageInfoGetter_ReturnsImageInfoOfInnerImageFile()
		{
			// Arrange

			var imageInfo = new ImageInfo();

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageInfo.Returns(imageInfo);

			var target = new DiscImageViewItem(new Disc(), DiscImageType.Cover, imageFileStub);

			// Act & Assert

			Assert.AreSame(imageInfo, target.ImageInfo);
		}
	}
}
