using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.UnitTests.Extensions;
using PandaPlayer.ViewModels.DiscImages;

namespace PandaPlayer.UnitTests.ViewModels.DiscImages
{
	[TestClass]
	public class DiscImageViewModelTests
	{
		[TestInitialize]
		public void Initialize()
		{
			Messenger.Reset();
		}

		[TestMethod]
		public void CoverImageSourceGetter_IfNoDiscIsSet_ReturnsNull()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			// Act

			var currentImageUri = target.CoverImageSource;

			// Assert

			currentImageUri.Should().BeNull();
		}

		[TestMethod]
		public void CoverImageSourceGetter_ForDeletedDisc_ReturnsImageSourceForDeletedDisc()
		{
			// Arrange

			var disc = new DiscModel().MakeDeleted();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			var currentImageUri = target.CoverImageSource;

			// Assert

			currentImageUri.Should().Be(DiscImageSource.ForDeletedDisc);
		}

		[TestMethod]
		public void CoverImageSourceGetter_ForDiscWithoutCoverImage_ReturnsNull()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			var currentImageUri = target.CoverImageSource;

			// Assert

			currentImageUri.Should().BeNull();
		}

		[TestMethod]
		public void CoverImageSourceGetter_IfCoverImageIsMissing_ReturnsImageSourceForMissingImage()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();
			disc.AddImage(new DiscImageModel { ImageType = DiscImageType.Cover, ContentUri = new Uri("file://Some-Missing-Image.jpg") });

			var fileSystemFacade = new Mock<IFileSystemFacade>();
			fileSystemFacade.Setup(x => x.FileExists("file://Some-Missing-Image.jpg")).Returns(false);

			var mocker = new AutoMocker();
			mocker.Use(fileSystemFacade);

			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			var currentImageUri = target.CoverImageSource;

			// Assert

			currentImageUri.Should().Be(DiscImageSource.ForMissingImage);
		}

		[TestMethod]
		public void CoverImageSourceGetter_ForExistingCoverImage_ReturnsImageSourceWithCoverFilePath()
		{
			// Arrange

			var disc = new DiscModel().MakeActive();
			disc.AddImage(new DiscImageModel { ImageType = DiscImageType.Cover, ContentUri = new Uri("file://Some-Existing-Image.jpg") });

			var fileSystemFacade = new Mock<IFileSystemFacade>();
			fileSystemFacade.Setup(x => x.FileExists("file://Some-Existing-Image.jpg")).Returns(true);

			var mocker = new AutoMocker();
			mocker.Use(fileSystemFacade);

			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			// Act

			var currentImageUri = target.CoverImageSource;

			// Assert

			currentImageUri.Should().BeEquivalentTo(DiscImageSource.ForImage(new Uri("file://Some-Existing-Image.jpg")));
		}

		[TestMethod]
		public void ActiveDiscChangedEventHandler_RaisesPropertyChangedEventForCoverImageSource()
		{
			// Arrange

			var disc1 = new DiscModel().MakeActive();
			disc1.AddImage(new DiscImageModel { ImageType = DiscImageType.Cover, ContentUri = new Uri("file://image1.jpg") });

			var disc2 = new DiscModel().MakeActive();
			disc2.AddImage(new DiscImageModel { ImageType = DiscImageType.Cover, ContentUri = new Uri("file://image2.jpg") });

			var fileSystemFacade = new Mock<IFileSystemFacade>();
			fileSystemFacade.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

			var mocker = new AutoMocker();
			mocker.Use(fileSystemFacade);

			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc1));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc2));

			// Assert

			propertyChangedEvents.Select(e => e.PropertyName).Should().Contain(nameof(DiscImageViewModel.CoverImageSource));
			target.CoverImageSource.Should().BeEquivalentTo(DiscImageSource.ForImage(new Uri("file://image2.jpg")));
		}

		[TestMethod]
		public void DiscImageChangedEventHandler_ForCurrentDisc_RaisesPropertyChangedEventForCoverImageSource()
		{
			// Arrange

			var disc = new DiscModel { Id = new ItemId("1") }.MakeActive();
			disc.AddImage(new DiscImageModel { ImageType = DiscImageType.Cover, ContentUri = new Uri("file://image.jpg") });

			var fileSystemFacade = new Mock<IFileSystemFacade>();
			fileSystemFacade.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

			var mocker = new AutoMocker();
			mocker.Use(fileSystemFacade);

			var target = mocker.CreateInstance<DiscImageViewModel>();

			Messenger.Default.Send(new ActiveDiscChangedEventArgs(disc));

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			Messenger.Default.Send(new DiscImageChangedEventArgs(new DiscModel { Id = new ItemId("1") }));

			// Assert

			propertyChangedEvents.Select(e => e.PropertyName).Should().Contain(nameof(DiscImageViewModel.CoverImageSource));
			target.CoverImageSource.Should().BeEquivalentTo(DiscImageSource.ForImage(new Uri("file://image.jpg")));
		}
	}
}
