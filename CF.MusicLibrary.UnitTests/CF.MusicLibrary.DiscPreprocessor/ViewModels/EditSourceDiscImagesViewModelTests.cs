using System;
using System.Linq;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.DiscPreprocessor;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	[TestFixture]
	public class EditSourceDiscImagesViewModelTests
	{
		[Test]
		public void Constructor_IfContentCrawlerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditSourceDiscImagesViewModel(null, Substitute.For<IObjectFactory<IImageFile>>()));
		}

		[Test]
		public void Constructor_IfImageFileFactoryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditSourceDiscImagesViewModel(Substitute.For<IContentCrawler>(), null));
		}

		[Test]
		public void LoadImages_FillsImageItemsCorrectly()
		{
			//	Arrange

			var disc = new Disc();
			var imageInfo = new ImageInfo();

			var addedDisc = new AddedDisc(disc, true, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(new[] { "ImagePath" });

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageInfo.Returns(imageInfo);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);

			//	Act

			target.LoadImages(new[] { addedDisc });

			//	Assert

			var imageItems = target.ImageItems;
			Assert.AreEqual(1, imageItems.Count);
			Assert.AreSame(disc, imageItems[0].Disc);
			Assert.AreEqual(DiscImageType.Cover, imageItems[0].ImageType);
			Assert.AreSame(imageInfo, imageItems[0].ImageInfo);
		}

		[Test]
		public void LoadImages_IfDiscHasImage_LoadsImageFileCorrectly()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), true, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(new[] { "ImagePath" });

			IImageFile imageFileMock = Substitute.For<IImageFile>();
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileMock);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);

			//	Act

			target.LoadImages(new[] { addedDisc });

			//	Assert

			imageFileMock.Received(1).Load("ImagePath", false);
		}

		[Test]
		public void LoadImages_ForExistingDiscs_DoesNotLoadImages()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), false, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(new[] { "ImagePath" });

			IImageFile imageFileMock = Substitute.For<IImageFile>();
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileMock);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);

			//	Act

			target.LoadImages(new[] { addedDisc });

			//	Assert

			Assert.IsEmpty(target.ImageItems);
			imageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void LoadImages_IfDiscHasNoImage_DoesNotLoadImageFile()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), true, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(Enumerable.Empty<string>());

			IImageFile imageFileMock = Substitute.For<IImageFile>();
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileMock);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);

			//	Act

			target.LoadImages(new[] { addedDisc });

			//	Assert

			imageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void LoadImages_IfDiscHasMultipleImages_ThrowsInvalidOperationException()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), true, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(new[] { "ImagePath1", "ImagePath2" });

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, Substitute.For<IObjectFactory<IImageFile>>());

			//	Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.LoadImages(new[] { addedDisc }));
		}

		[Test]
		public void LoadImages_IfSomeImagesAreAlreadyLoaded_ClearsExistingImages()
		{
			//	Arrange

			var oldAddedDisc = new AddedDisc(new Disc(), true, "OldDiscSourcePath");
			var newAddedDisc = new AddedDisc(new Disc(), true, "NewDiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages(Arg.Any<string>()).Returns(new[] { "SomeImage.img" });

			var imageInfo1 = new ImageInfo();
			var imageInfo2 = new ImageInfo();
			IImageFile imageFileStub1 = Substitute.For<IImageFile>();
			imageFileStub1.ImageInfo.Returns(imageInfo1);
			IImageFile imageFileStub2 = Substitute.For<IImageFile>();
			imageFileStub2.ImageInfo.Returns(imageInfo2);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub1, imageFileStub2);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { oldAddedDisc });
			//	Sanity check
			Assert.AreSame(imageInfo1, target.ImageItems.Single().ImageInfo);

			//	Act

			target.LoadImages(new[] { newAddedDisc });

			//	Assert

			Assert.AreEqual(1, target.ImageItems.Count);
			Assert.AreSame(imageInfo2, target.ImageItems.Single().ImageInfo);
		}

		[Test]
		public void RefreshContentCommand_ReloadsDiscImages()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), true, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages(Arg.Any<string>()).Returns(new[] { "SomeImage.img" });

			var imageInfo1 = new ImageInfo();
			var imageInfo2 = new ImageInfo();
			IImageFile imageFileStub1 = Substitute.For<IImageFile>();
			imageFileStub1.ImageInfo.Returns(imageInfo1);
			IImageFile imageFileStub2 = Substitute.For<IImageFile>();
			imageFileStub2.ImageInfo.Returns(imageInfo2);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub1, imageFileStub2);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { addedDisc });
			//	Sanity check
			Assert.AreSame(imageInfo1, target.ImageItems.Single().ImageInfo);

			//	Act

			target.RefreshContent();

			//	Assert

			Assert.AreEqual(1, target.ImageItems.Count);
			Assert.AreSame(imageInfo2, target.ImageItems.Single().ImageInfo);
		}

		[Test]
		public void RefreshContentCommand_ForExistingDiscs_DoesNotLoadImages()
		{
			//	Arrange

			var addedDisc = new AddedDisc(new Disc(), false, "DiscSourcePath");

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages("DiscSourcePath").Returns(new[] { "ImagePath" });

			IImageFile imageFileMock = Substitute.For<IImageFile>();
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileMock);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { addedDisc });
			imageFileMock.ClearReceivedCalls();

			//	Act

			target.RefreshContent();

			//	Assert

			Assert.IsEmpty(target.ImageItems);
			imageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void DataIsReadyGetter_IfAllImagesAreValid_ReturnsTrue()
		{
			//	Arrange

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages(Arg.Any<string>()).Returns(new[] { "SomeImage" });

			IImageFile imageFileStub1 = Substitute.For<IImageFile>();
			imageFileStub1.ImageIsValid.Returns(true);
			IImageFile imageFileStub2 = Substitute.For<IImageFile>();
			imageFileStub2.ImageIsValid.Returns(true);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub1, imageFileStub2);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { new AddedDisc(new Disc(), true, "DiscSourcePath1"), new AddedDisc(new Disc(), true, "DiscSourcePath2") });

			//	Act & Assert

			Assert.IsTrue(target.DataIsReady);
		}

		[Test]
		public void DataIsReadyGetter_IfSomeImagesAreNotValid_ReturnsTrue()
		{
			//	Arrange

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages(Arg.Any<string>()).Returns(new[] { "SomeImage" });

			IImageFile imageFileStub1 = Substitute.For<IImageFile>();
			imageFileStub1.ImageIsValid.Returns(true);
			IImageFile imageFileStub2 = Substitute.For<IImageFile>();
			imageFileStub2.ImageIsValid.Returns(false);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub1, imageFileStub2);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { new AddedDisc(new Disc(), true, "DiscSourcePath1"), new AddedDisc(new Disc(), true, "DiscSourcePath2") });

			//	Act & Assert

			Assert.IsFalse(target.DataIsReady);
		}

		[Test]
		public void AddedImagesGetter_ReturnsCorrectAddedDiscImageCollection()
		{
			//	Arrange

			var disc1 = new Disc();
			var imageInfo1 = new ImageInfo();
			var disc2 = new Disc();
			var imageInfo2 = new ImageInfo();

			IContentCrawler contentCrawlerStub = Substitute.For<IContentCrawler>();
			contentCrawlerStub.LoadDiscImages(Arg.Any<string>()).Returns(new[] { "SomeImage" });

			IImageFile imageFileStub1 = Substitute.For<IImageFile>();
			imageFileStub1.ImageInfo.Returns(imageInfo1);
			IImageFile imageFileStub2 = Substitute.For<IImageFile>();
			imageFileStub2.ImageInfo.Returns(imageInfo2);
			IObjectFactory<IImageFile> imageFileFactoryStub = Substitute.For<IObjectFactory<IImageFile>>();
			imageFileFactoryStub.CreateInstance().Returns(imageFileStub1, imageFileStub2);

			var target = new EditSourceDiscImagesViewModel(contentCrawlerStub, imageFileFactoryStub);
			target.LoadImages(new[] { new AddedDisc(disc1, true, "DiscSourcePath1"), new AddedDisc(disc2, true, "DiscSourcePath2") });

			//	Act

			var addedImages = target.AddedImages.ToList();

			//Assert

			Assert.AreEqual(2, addedImages.Count);
			Assert.AreSame(disc1, addedImages[0].Disc);
			Assert.AreSame(imageInfo1, addedImages[0].ImageInfo);
			Assert.AreSame(disc2, addedImages[1].Disc);
			Assert.AreSame(imageInfo2, addedImages[1].ImageInfo);
		}
	}
}
