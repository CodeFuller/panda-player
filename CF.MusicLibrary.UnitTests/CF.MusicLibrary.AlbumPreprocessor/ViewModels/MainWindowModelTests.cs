using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	[TestFixture]
	public class MainWindowModelTests
	{
		[TearDown]
		public void TearDown()
		{
			AppSettings.ResetSettingsProvider();
		}

		[Test]
		public void AddToLibraryCommand_AfterAddingAlbumsToLibrary_DeletesSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			settingsProvider.GetRequiredValue<bool>("DeleteSourceContentAfterAdding").Returns(true);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir1", @"SomeWorkshopDirectory\SubDir2" });

			EditAlbumsDetailsViewModel editAlbumsDetailsViewModelStub = Substitute.For<EditAlbumsDetailsViewModel>(
				Substitute.For<IMusicLibrary>(), Substitute.For<IWorkshopMusicStorage>(),
				Substitute.For<IStorageUrlBuilder>(), Substitute.For<IFileSystemFacade>());

			AddToLibraryViewModel addToLibraryViewModelStub = Substitute.For<AddToLibraryViewModel>(editAlbumsDetailsViewModelStub, Substitute.For<EditSongsDetailsViewModel>(), 
				Substitute.For<ISongTagger>(), Substitute.For<IWindowService>(), Substitute.For<IMusicLibrary>(), Substitute.For<IFileSystemFacade>());

			IObjectFactory<AddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<AddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(addToLibraryViewModelStub);

			MainWindowModel target = new MainWindowModel(fileSystemMock, Substitute.For<IAlbumContentParser>(), Substitute.For<IAlbumContentComparer>(),
				addToLibraryViewModelFactory);

			//	Act

			target.AddToLibraryCommand.Execute(null);

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir1", true);
			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir2", true);
		}

		[Test]
		public void AddToLibraryCommand_IfSomeSubDirectoryContainsFiles_DoesNotDeleteSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			settingsProvider.GetRequiredValue<bool>("DeleteSourceContentAfterAdding").Returns(true);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir" });
			fileSystemMock.EnumerateDirectories(@"SomeWorkshopDirectory\SubDir").Returns(new[] { @"SomeWorkshopDirectory\SubDir\DeeperDir" });
			fileSystemMock.EnumerateFiles(@"SomeWorkshopDirectory\SubDir\DeeperDir").Returns(new[] { @"SomeWorkshopDirectory\SubDir\DeeperDir\SomeFile.mp3" });

			EditAlbumsDetailsViewModel editAlbumsDetailsViewModelStub = Substitute.For<EditAlbumsDetailsViewModel>(
				Substitute.For<IMusicLibrary>(), Substitute.For<IWorkshopMusicStorage>(),
				Substitute.For<IStorageUrlBuilder>(), Substitute.For<IFileSystemFacade>());

			AddToLibraryViewModel addToLibraryViewModelStub = Substitute.For<AddToLibraryViewModel>(editAlbumsDetailsViewModelStub, Substitute.For<EditSongsDetailsViewModel>(),
				Substitute.For<ISongTagger>(), Substitute.For<IWindowService>(), Substitute.For<IMusicLibrary>(), Substitute.For<IFileSystemFacade>());

			IObjectFactory<AddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<AddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(addToLibraryViewModelStub);

			MainWindowModel target = new MainWindowModel(fileSystemMock, Substitute.For<IAlbumContentParser>(), Substitute.For<IAlbumContentComparer>(),
				addToLibraryViewModelFactory);

			//	Act

			target.AddToLibraryCommand.Execute(null);

			//	Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(@"SomeWorkshopDirectory\SubDir", true);
		}

		[Test]
		public void AddToLibraryCommand_IfDeleteSourceContentAfterAddingIsFalse_DoesNotDeleteSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			settingsProvider.GetRequiredValue<bool>("DeleteSourceContentAfterAdding").Returns(false);
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir" });

			EditAlbumsDetailsViewModel editAlbumsDetailsViewModelStub = Substitute.For<EditAlbumsDetailsViewModel>(
				Substitute.For<IMusicLibrary>(), Substitute.For<IWorkshopMusicStorage>(),
				Substitute.For<IStorageUrlBuilder>(), Substitute.For<IFileSystemFacade>());

			AddToLibraryViewModel addToLibraryViewModelStub = Substitute.For<AddToLibraryViewModel>(editAlbumsDetailsViewModelStub, Substitute.For<EditSongsDetailsViewModel>(),
				Substitute.For<ISongTagger>(), Substitute.For<IWindowService>(), Substitute.For<IMusicLibrary>(), Substitute.For<IFileSystemFacade>());

			IObjectFactory<AddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<AddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(addToLibraryViewModelStub);

			MainWindowModel target = new MainWindowModel(fileSystemMock, Substitute.For<IAlbumContentParser>(), Substitute.For<IAlbumContentComparer>(),
				addToLibraryViewModelFactory);

			//	Act

			target.AddToLibraryCommand.Execute(null);

			//	Assert

			fileSystemMock.DidNotReceiveWithAnyArgs().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
		}
	}
}
