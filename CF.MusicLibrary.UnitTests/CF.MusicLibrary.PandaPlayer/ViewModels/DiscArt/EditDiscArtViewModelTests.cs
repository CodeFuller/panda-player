using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	[TestFixture]
	public class EditDiscArtViewModelTests
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
			Assert.Throws<ArgumentNullException>(() => new EditDiscArtViewModel(null, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfDocumentDownloaderArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), null,
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfDiscArtValidatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				null, Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), null, Substitute.For<IWebBrowser>()));
		}

		[Test]
		public void Constructor_IfWebBrowserArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), null));
		}

		[Test]
		public void Load_SetsDiscProperty()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.Load(disc).Wait();

			//	Assert

			Assert.AreSame(disc, target.Disc);
		}

		[Test]
		public void Load_UnloadsPreviousDiscArtImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.Load(new Disc()).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Unload();
		}

		[Test]
		public void Load_WhenDiscHasNoDiscCover_DoesNotLoadDiscArtImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult((string)null));

			var target = new EditDiscArtViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.Load(disc).Wait();

			//	Assert

			discArtImageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void Load_WhenDiscHasDiscCover_LoadDiscArtImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			IMusicLibrary musicLibraryStub = Substitute.For<IMusicLibrary>();
			musicLibraryStub.GetDiscCoverImage(disc).Returns(Task.FromResult("SomeExistingCover.jpg"));

			var target = new EditDiscArtViewModel(musicLibraryStub, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.Load(disc).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load("SomeExistingCover.jpg", false);
		}

		[Test]
		public void Load_SetsImageWasChangedToFalse()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();
			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

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
		public void Load_IfSearchedDiscDataIsFilled_OpensDiscCoverImageLookupPagesCorrectly()
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

			IWebBrowser webBrowserMock = Substitute.For<IWebBrowser>();

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), webBrowserMock);

			//	Act

			target.Load(disc).Wait();

			//	Assert

			webBrowserMock.Received(1).OpenPage(new Uri("http://google.com?search1=\"Some Artist\" and \"Some Album\""));
			webBrowserMock.Received(1).OpenPage(new Uri("http://google.com?search2=\"Some Artist\" and \"Some Album\""));
		}

		[Test]
		public void Unload_SetsDiscPropertyToNull()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			target.Load(new Disc()).Wait();
			//	Paranoic check
			Assert.IsNotNull(target.Disc);

			//	Act

			target.Unload();

			//	Assert

			Assert.IsNull(target.Disc);
		}

		[Test]
		public void Unload_UnloadsPreviousDiscArtImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.Unload();

			//	Assert

			discArtImageFileMock.Received(1).Unload();
		}

		[Test]
		public void Save_IfViewModelIsNotLoaded_Throws()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

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

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(false);
			target.DiscArtImageFile = discArtImageFileStub;

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

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			target.DiscArtImageFile = discArtImageFileStub;

			target.Load(new Disc()).Wait();

			//	Act & Assert

			Assert.Throws<AggregateException>(() => target.Save().Wait());
		}

		[Test]
		public void Save_IfImageIsValid_UpdatesDiscCoverImageInLibrary()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();

			var target = new EditDiscArtViewModel(musicLibraryMock, Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageFileName.Returns("SomeImageCover.jpg");
			target.DiscArtImageFile = discArtImageFileStub;

			target.Load(disc).Wait();
			//	Setting ImageWasChanged to true
			target.SetImage(new byte[] { });

			//	Act

			target.Save().Wait();

			//	Assert

			musicLibraryMock.Received(1).SetDiscCoverImage(disc, "SomeImageCover.jpg");
		}

		[Test]
		public void Save_IfImageIsValid_SendsDiscArtChangedEvent()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var disc = new Disc();

			bool receivedEvent = false;
			Messenger.Default.Register<DiscArtChangedEventArgs>(this, e => receivedEvent = (e.Disc == disc));

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			target.DiscArtImageFile = discArtImageFileStub;

			target.Load(disc).Wait();
			//	Setting ImageWasChanged to true
			target.SetImage(new byte[] { });

			//	Act

			target.Save().Wait();

			//	Assert

			Assert.IsTrue(receivedEvent);
		}

		[Test]
		public void SetImage1_WhenUriPointsToExistingLocalFile_LoadsDiscArtImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists("c:\\SomeCover.jpg").Returns(true);

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemStub, Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load("c:\\SomeCover.jpg", false);
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

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemStub, Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			Assert.IsTrue(target.ImageWasChanged);
		}

		[Test]
		public void SetImage1_WhenUriPointsToUnexistingLocalFile_DoesNotLoadDiscArtImageFile()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists("c:\\SomeCover.jpg").Returns(false);

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemStub, Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.SetImage(new Uri("c:\\SomeCover.jpg")).Wait();

			//	Assert

			discArtImageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
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

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), documentDownloaderStub,
				Substitute.For<IDiscArtValidator>(), fileSystemFacadeMock, Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new Uri("http://www.test.com/")).Wait();

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllBytes("SomeTempFile.tmp", downloadedData);
		}

		[Test]
		public void SetImage1_WhenUriIsRemote_LoadsDiscArtImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.GetTempFileName().Returns("SomeTempFile.tmp");

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemFacadeStub, Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			//	Act

			target.SetImage(new Uri("http://www.test.com/")).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load("SomeTempFile.tmp", true);
		}

		[Test]
		public void SetImage1_WhenUriIsRemote_SetsImageWasChangedToTrue()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

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

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemFacadeMock, Substitute.For<IWebBrowser>());

			byte[] imageData = { };

			//	Act

			target.SetImage(imageData);

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllBytes("SomeTempFile.tmp", imageData);
		}

		[Test]
		public void SetImage2_LoadsDiscArtImageFileCorrectly()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.GetTempFileName().Returns("SomeTempFile.tmp");

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), fileSystemFacadeStub, Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileMock;

			byte[] imageData = { };

			//	Act

			target.SetImage(imageData);

			//	Assert

			discArtImageFileMock.Received(1).Load("SomeTempFile.tmp", true);
		}

		[Test]
		public void SetImage2_SetsImageWasChangedToTrue()
		{
			//	Arrange

			var settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetOptionalValue<string>("DiscCoverImageLookupPages1").Returns((string)null);
			AppSettings.SettingsProvider = settingsProvider;

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			//	Act

			target.SetImage(new byte[] {});

			//	Assert

			Assert.IsTrue(target.ImageWasChanged);
		}

		[Test]
		public void PropertiesGetters_ReturnCorrespondingPropertiesFromDiscArtImageFile()
		{
			//	Arrange

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileStub;

			discArtImageFileStub.ImageFileName.Returns("SomeImageFileName");
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageProperties.Returns("SomeImageProperties");
			discArtImageFileStub.ImageStatus.Returns("SomeImageStatus");

			//	Act & Assert

			Assert.AreEqual("SomeImageFileName", target.ImageFileName);
			Assert.AreEqual(true, target.ImageIsValid);
			Assert.AreEqual("SomeImageProperties", target.ImageProperties);
			Assert.AreEqual("SomeImageStatus", target.ImageStatus);
		}

		[Test]
		public void ImageFileName_WhenDiscArtImageFilePropertyIsChanged_RaisesPropertyChangedEventCorrectly()
		{
			//	Arrange

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileStub;

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(discArtImageFileStub, new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageFileName)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscArtViewModel.ImageFileName), changedPropertyName);
		}

		[Test]
		public void ImageIsValid_WhenDiscArtImageFilePropertyIsChanged_RaisesPropertyChangedEventCorrectly()
		{
			//	Arrange

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileStub;

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(discArtImageFileStub, new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageIsValid)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscArtViewModel.ImageIsValid), changedPropertyName);
		}

		[Test]
		public void ImageProperties_WhenDiscArtImageFilePropertyIsChanged_RaisesPropertyChangedEventCorrectly()
		{
			//	Arrange

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileStub;

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(discArtImageFileStub, new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageProperties)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscArtViewModel.ImageProperties), changedPropertyName);
		}

		[Test]
		public void ImageStatus_WhenDiscArtImageFilePropertyIsChanged_RaisesPropertyChangedEventCorrectly()
		{
			//	Arrange

			var target = new EditDiscArtViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<IDocumentDownloader>(),
				Substitute.For<IDiscArtValidator>(), Substitute.For<IFileSystemFacade>(), Substitute.For<IWebBrowser>());

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			target.DiscArtImageFile = discArtImageFileStub;

			string changedPropertyName = null;
			target.PropertyChanged += (sender, e) => changedPropertyName = e.PropertyName;

			//	Act

			discArtImageFileStub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(discArtImageFileStub, new PropertyChangedEventArgs(nameof(IDiscArtImageFile.ImageStatus)));

			//	Assert

			Assert.AreEqual(nameof(EditDiscArtViewModel.ImageStatus), changedPropertyName);
		}
	}
}
