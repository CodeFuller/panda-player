using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
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

			IObjectFactory<IAddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<IAddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(Substitute.For<IAddToLibraryViewModel>());

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

			IObjectFactory<IAddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<IAddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(Substitute.For<IAddToLibraryViewModel>());

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

			IObjectFactory<IAddToLibraryViewModel> addToLibraryViewModelFactory = Substitute.For<IObjectFactory<IAddToLibraryViewModel>>();
			addToLibraryViewModelFactory.CreateInstance().Returns(Substitute.For<IAddToLibraryViewModel>());

			MainWindowModel target = new MainWindowModel(fileSystemMock, Substitute.For<IAlbumContentParser>(), Substitute.For<IAlbumContentComparer>(),
				addToLibraryViewModelFactory);

			//	Act

			target.AddToLibraryCommand.Execute(null);

			//	Assert

			fileSystemMock.DidNotReceiveWithAnyArgs().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
		}
	}
}
