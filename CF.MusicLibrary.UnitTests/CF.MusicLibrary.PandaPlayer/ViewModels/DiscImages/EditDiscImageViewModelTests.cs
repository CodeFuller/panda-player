using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	[TestFixture]
	public class EditDiscImageViewModelTests
	{
		[SetUp]
		public void SetUp()
		{
			Messenger.Reset();
		}

		[TearDown]
		public void TearDown()
		{
			AppSettings.ResetSettingsProvider();
		}

		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscImageViewModel(null, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfDocumentDownloaderArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), null,
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfImageFileArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				null, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), null, Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfWebBrowserArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), null));
		}

		[Test]
		public void Load_SetsDiscProperty()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Load(disc).Wait();

			//	Assert

			Assert.AreSame(disc, target.Disc);
		}

		[Test]
		public void Load_UnloadsPreviousImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Load(new Disc()).Wait();

			//	Assert

			imageFileMock.Received(1).Unload();
		}

		[Test]
		public void Load_WhenDiscHasNoDiscCover_DoesNotLoadDiscCoverImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult((string)null));

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				imageFileMock, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Load(disc).Wait();

			//	Assert

			imageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void Load_WhenDiscHasDiscCover_LoadDiscCoverImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult("SomeExistingCover.jpg"));

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				imageFileMock, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Load(disc).Wait();

			//	Assert

			imageFileMock.Received(1).Load("SomeExistingCover.jpg", false);
		}

		[Test]
		public void Load_SetsImageWasChangedToFalse()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(disc).Wait();
			target.SetImage(new byte[] {});
			//	Paranoic check.
			Assert.IsTrue(target.ImageWasChanged);

			//	Act

			target.Load(disc).Wait();

			//	Assert

			Assert.IsFalse(target.ImageWasChanged);
		}

		[Test]
		public void Load_IfDiscHasNoCoverAndDiscCoverImageLookupPagesAreConfigured_OpensDiscCoverImageLookupPagesCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns("http://google.com?search1=\"{DiscArtist}\" and \"{DiscTitle}\"");
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages2").Returns("http://google.com?search2=\"{DiscArtist}\" and \"{DiscTitle}\"");
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages3").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc
			{
				AlbumTitle = "Some Album",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist" } } },
			};

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult((string)null));

			IWebBrowser webBrowserMock = Substitute.For<IWebBrowser>();

			var target = new EditDiscImageViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), webBrowserMock);

			//	Act

			target.Load(disc).Wait();

			//	Assert

			webBrowserMock.Received(1).OpenPage("http://google.com?search1=\"Some%20Artist\" and \"Some%20Album\"");
			webBrowserMock.Received(1).OpenPage("http://google.com?search2=\"Some%20Artist\" and \"Some%20Album\"");
		}

		[Test]
		public void Load_WhenDiscCoverImageLookupPageIsOpened_EscapesSpecialCharactersInPastedData()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns("http://google.com?search=\"{DiscArtist}\" and \"{DiscTitle}\"");
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages2").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc
			{
				AlbumTitle = "Some Album",
				SongsUnordered = new[] { new Song { Artist = new Artist { Name = "Some Artist & Co" } } },
			};

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult((string)null));

			IWebBrowser webBrowserMock = Substitute.For<IWebBrowser>();

			var target = new EditDiscImageViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), webBrowserMock);

			//	Act

			target.Load(disc).Wait();

			//	Assert

			webBrowserMock.Received(1).OpenPage("http://google.com?search=\"Some%20Artist%20%26%20Co\" and \"Some%20Album\"");
		}

		[Test]
		public void Load_IfDiscHasCover_DoesNotOpenDiscCoverImageLookupPages()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns("http://google.com?search1=\"{DiscArtist}\" and \"{DiscTitle}\"");
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages2").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(Arg.Any<Disc>()).Returns(Task.FromResult("cover.jpg"));

			IWebBrowser webBrowserMock = Substitute.For<IWebBrowser>();

			var target = new EditDiscImageViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), webBrowserMock);

			//	Act

			target.Load(new Disc()).Wait();

			//	Assert

			webBrowserMock.DidNotReceive().OpenPage(Arg.Any<string>());
		}

		[Test]
		public void Unload_SetsDiscPropertyToNull()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(new Disc()).Wait();
			//	Paranoic check
			Assert.IsNotNull(target.Disc);

			//	Act

			target.Unload();

			//	Assert

			Assert.IsNull(target.Disc);
		}

		[Test]
		public void Unload_UnloadsPreviousImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Unload();

			//	Assert

			imageFileMock.Received(1).Unload();
		}

		[Test]
		public void Save_IfViewModelIsNotLoaded_Throws()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act & Assert

			Assert.Throws<AggregateException>(() => target.Save().Wait());
		}

		[Test]
		public void Save_IfImageIsNotValid_Throws()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(false);

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(new Disc()).Wait();
			//	Setting ImageWasChanged to true
			target.SetImage(new byte[] { });

			//	Act & Assert

			Assert.Throws<AggregateException>(() => target.Save().Wait());
		}

		[Test]
		public void Save_IfImageWasNotChanged_Throws()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(true);

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(new Disc()).Wait();

			//	Act & Assert

			Assert.Throws<AggregateException>(() => target.Save().Wait());
		}

		[Test]
		public void Save_IfImageIsValid_UpdatesDiscCoverImageInLibrary()
		{
			//	Arrange

			ImageInfo imageInfo = new ImageInfo();

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(true);
			imageFileStub.ImageInfo.Returns(imageInfo);

			var target = new EditDiscImageViewModel(musicLibraryMock, Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(disc).Wait();
			//	Setting ImageWasChanged to true.
			target.SetImage(new byte[] { });

			//	Act

			target.Save().Wait();

			//	Assert

			musicLibraryMock.Received(1).SetDiscCoverImage(disc, imageInfo);
		}

		[Test]
		public void Save_IfImageIsValid_SendsDiscImageChangedEvent()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageIsValid.Returns(true);

			bool receivedEvent = false;
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => receivedEvent = (e.Disc == disc));

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(disc).Wait();
			//	Setting ImageWasChanged to true
			target.SetImage(new byte[] { });

			//	Act

			target.Save().Wait();

			//	Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void SetImage1_WhenUriPointsToExistingLocalFile_LoadsImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists("c:\\SomeCover.jpg").Returns(true);

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, fileSystemStub, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			imageFileMock.Received(1).Load("c:\\SomeCover.jpg", false);
		}

		[Test]
		public void SetImage1_WhenUriPointsToExistingLocalFile_SetsImageWasChangedToTrue()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists("c:\\SomeCover.jpg").Returns(true);

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, fileSystemStub, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			Assert.IsTrue(target.ImageWasChanged);
		}

		[Test]
		public void SetImage1_WhenUriPointsToUnexistingLocalFile_DoesNotLoadImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists("c:\\SomeCover.jpg").Returns(false);

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, fileSystemStub, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			imageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void SetImage1_WhenUriIsRemote_DownloadsDataToTemporaryFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			byte[] downloadedData = { };
			var documentDownloaderStub = Substitute.For<IDocumentDownloader>();
			documentDownloaderStub.Download(new Uri("http://www.test.com/")).Returns(Task.FromResult(downloadedData));

			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.GetTempFileName().Returns("SomeTempFile.tmp");

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), documentDownloaderStub,
				Substitute.For<IImageFile>(), fileSystemFacadeMock, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("http://www.test.com/")).Wait();

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllBytes("SomeTempFile.tmp", downloadedData);
		}

		[Test]
		public void SetImage1_WhenUriIsRemote_LoadsImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.GetTempFileName().Returns("SomeTempFile.tmp");

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, fileSystemFacadeStub, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("http://www.test.com/")).Wait();

			//	Assert

			imageFileMock.Received(1).Load("SomeTempFile.tmp", true);
		}

		[Test]
		public void SetImage1_WhenUriIsRemote_SetsImageWasChangedToTrue()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("http://www.test.com/")).Wait();

			//	Assert

			Assert.IsTrue(target.ImageWasChanged);
		}

		[Test]
		public void SetImage2_SavesDataToTemporaryFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.GetTempFileName().Returns("SomeTempFile.tmp");

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), fileSystemFacadeMock, Substitute.For<IWebBrowser>());

			byte[] imageData = { };

			//	Act

			target.SetImage(imageData);

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllBytes("SomeTempFile.tmp", imageData);
		}

		[Test]
		public void SetImage2_LoadsImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.GetTempFileName().Returns("SomeTempFile.tmp");

			IImageFile imageFileMock = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileMock, fileSystemFacadeStub, Substitute.For<IWebBrowser>());

			byte[] imageData = { };

			//	Act

			target.SetImage(imageData);

			//	Assert

			imageFileMock.Received(1).Load("SomeTempFile.tmp", true);
		}

		[Test]
		public void SetImage2_SetsImageWasChangedToTrue()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IImageFile>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new byte[] {});

			//	Assert

			Assert.IsTrue(target.ImageWasChanged);
		}

		[Test]
		public void PropertiesGetters_ReturnCorrespondingPropertiesFromImageFile()
		{
			//	Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();
			imageFileStub.ImageFileName.Returns("SomeImageFileName");
			imageFileStub.ImageIsValid.Returns(true);
			imageFileStub.ImageProperties.Returns("SomeImageProperties");
			imageFileStub.ImageStatus.Returns("SomeImageStatus");

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act & Assert

			Assert.AreEqual("SomeImageFileName", target.ImageFileName);
			Assert.AreEqual(true, target.ImageIsValid);
			Assert.AreEqual("SomeImageProperties", target.ImageProperties);
			Assert.AreEqual("SomeImageStatus", target.ImageStatus);
		}

		[Test]
		public void ImageFilePropertyChangedEventHandler_WhenImageFileNamePropertyIsChanged_RaisesPropertyChangedEventForImageFileNameProperty()
		{
			//	Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			imageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(imageFileStub, new PropertyChangedEventArgs(nameof(IImageFile.ImageFileName)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscImageViewModel.ImageFileName), changedPropertyName);
		}

		[Test]
		public void ImageFilePropertyChangedEventHandler_WhenImageIsValidPropertyIsChanged_RaisesPropertyChangedEventForImageIsValidProperty()
		{
			//	Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			imageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(imageFileStub, new PropertyChangedEventArgs(nameof(IImageFile.ImageIsValid)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscImageViewModel.ImageIsValid), changedPropertyName);
		}

		[Test]
		public void ImageFilePropertyChangedEventHandler_WhenImagePropertiesPropertyIsChanged_RaisesPropertyChangedEventForImagePropertiesProperty()
		{
			//	Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			imageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(imageFileStub, new PropertyChangedEventArgs(nameof(IImageFile.ImageProperties)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscImageViewModel.ImageProperties), changedPropertyName);
		}

		[Test]
		public void ImageFilePropertyChangedEventHandler_WhenImageStatusPropertyIsChanged_RaisesPropertyChangedEventForImageStatusProperty()
		{
			//	Arrange

			IImageFile imageFileStub = Substitute.For<IImageFile>();

			var target = new EditDiscImageViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				imageFileStub, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			imageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(imageFileStub, new PropertyChangedEventArgs(nameof(IImageFile.ImageStatus)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscImageViewModel.ImageStatus), changedPropertyName);
		}
	}
}
