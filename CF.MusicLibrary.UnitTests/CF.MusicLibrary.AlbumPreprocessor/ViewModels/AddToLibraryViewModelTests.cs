using System.Linq;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	[TestFixture]
	public class AddToLibraryViewModelTests
	{
		[TearDown]
		public void TearDown()
		{
			AppSettings.ResetSettingsProvider();
		}

		[Test]
		public void AddAlbumsToLibrary_ClearsReadOnlyAttributeBeforeTagging()
		{
			//	Arrange

			TaggedSongData songData = new TaggedSongData
			{
				SourceFileName = @"SomeSongPath\SomeSongFile.mp3"
			};

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			AppSettings.SettingsProvider = settingsProvider;

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<ISongTagger>(), Substitute.For<IMusicLibrary>(), fileSystemMock);
			target.SetSongsTagData(Enumerable.Repeat(songData, 1));

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.Received(1).ClearReadOnlyAttribute(@"SomeSongPath\SomeSongFile.mp3");
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

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<ISongTagger>(), Substitute.For<IMusicLibrary>(), fileSystemMock);
			target.SetSongsTagData(Enumerable.Repeat(new TaggedSongData { SourceFileName = @"SomeSongPath\SomeSongFile.mp3" }, 1));

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir1", true);
			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir2", true);
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
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir1", @"SomeWorkshopDirectory\SubDir2" });

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<ISongTagger>(), Substitute.For<IMusicLibrary>(), fileSystemMock);
			target.SetSongsTagData(Enumerable.Repeat(new TaggedSongData { SourceFileName = @"SomeSongPath\SomeSongFile.mp3" }, 1));

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.DidNotReceiveWithAnyArgs().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
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

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<ISongTagger>(), Substitute.For<IMusicLibrary>(), fileSystemMock);
			target.SetSongsTagData(Enumerable.Repeat(new TaggedSongData { SourceFileName = @"SomeSongPath\SomeSongFile.mp3" }, 1));

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(@"SomeWorkshopDirectory\SubDir", true);
		}
	}
}
