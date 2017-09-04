using System;
using System.Threading.Tasks;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
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
		public void AddContentToLibrary_FillsSongMediaInfoCorrectly()
		{
			//	Arrange

			Song addedSong = null;

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();
			musicLibraryMock.AddSong(Arg.Do<Song>(arg => addedSong = arg), Arg.Any<string>());

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(
			new SongMediaInfo
			{
				Size = 12345,
				Bitrate = 256000,
				Duration = TimeSpan.FromSeconds(3600),
			}));

			AddToLibraryViewModel target = new AddToLibraryViewModel(musicLibraryMock, mediaInfoProviderStub, Substitute.For<IFileSystemFacade>(), false);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			Assert.AreEqual(12345, addedSong.FileSize);
			Assert.AreEqual(256000, addedSong.Bitrate);
			Assert.AreEqual(TimeSpan.FromSeconds(3600), addedSong.Duration);
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsTrue_DeletesSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			AppSettings.SettingsProvider = settingsProvider;

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir1", @"SomeWorkshopDirectory\SubDir2" });

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), mediaInfoProviderStub, fileSystemMock, true);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir1", true);
			fileSystemMock.Received(1).DeleteDirectory(@"SomeWorkshopDirectory\SubDir2", true);
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsFalse_DoesNotDeleteSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			AppSettings.SettingsProvider = settingsProvider;

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir1", @"SomeWorkshopDirectory\SubDir2" });

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), mediaInfoProviderStub, fileSystemMock, false);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.DidNotReceiveWithAnyArgs().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void AddContentToLibrary_IfSomeSubDirectoryContainsFiles_DoesNotDeleteSourceDirTree()
		{
			//	Arrange

			ISettingsProvider settingsProvider = Substitute.For<ISettingsProvider>();
			settingsProvider.GetRequiredValue<string>("WorkshopDirectory").Returns("SomeWorkshopDirectory");
			AppSettings.SettingsProvider = settingsProvider;

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeWorkshopDirectory").Returns(new[] { @"SomeWorkshopDirectory\SubDir" });
			fileSystemMock.EnumerateDirectories(@"SomeWorkshopDirectory\SubDir").Returns(new[] { @"SomeWorkshopDirectory\SubDir\DeeperDir" });
			fileSystemMock.EnumerateFiles(@"SomeWorkshopDirectory\SubDir\DeeperDir").Returns(new[] { @"SomeWorkshopDirectory\SubDir\DeeperDir\SomeFile.mp3" });

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), mediaInfoProviderStub, fileSystemMock, true);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(@"SomeWorkshopDirectory\SubDir", true);
		}
	}
}
