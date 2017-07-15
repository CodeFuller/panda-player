using System;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.BL
{
	[TestFixture]
	public class FilesystemMusicStorageTests
	{
		[Test]
		public void AddSongAsync_CreatesDestinationDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FilesystemMusicStorage target = new FilesystemMusicStorage(fileSystemMock, "RootDir", false);

			//	Act

			target.AddSongAsync("SourceName.mp3", new Uri("/Foreign/SomeAlbum/DestinationName.mp3", UriKind.Relative)).Wait();

			//	Assert

			fileSystemMock.Received(1).CreateDirectory(@"RootDir\Foreign\SomeAlbum");
		}

		[Test]
		public void AddSongAsync_InMoveMode_MovesSongFilesCorrectly()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FilesystemMusicStorage target = new FilesystemMusicStorage(fileSystemMock, "RootDir", true);

			//	Act

			target.AddSongAsync(@"SomeSourceDir\SourceName.mp3", new Uri("/Foreign/SomeAlbum/DestinationName.mp3", UriKind.Relative)).Wait();

			//	Assert

			fileSystemMock.Received(1).MoveFile(@"SomeSourceDir\SourceName.mp3", @"RootDir\Foreign\SomeAlbum\DestinationName.mp3");
		}

		[Test]
		public void AddSongAsync_InCopyMode_CopiesSongFilesCorrectly()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FilesystemMusicStorage target = new FilesystemMusicStorage(fileSystemMock, "RootDir", false);

			//	Act

			target.AddSongAsync(@"SomeSourceDir\SourceName.mp3", new Uri("/Foreign/SomeAlbum/DestinationName.mp3", UriKind.Relative)).Wait();

			//	Assert

			fileSystemMock.Received(1).CopyFile(@"SomeSourceDir\SourceName.mp3", @"RootDir\Foreign\SomeAlbum\DestinationName.mp3");
		}

		[Test]
		public void AddSongAsync_SetsReadOnlyAttributeForDestinationSongFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FilesystemMusicStorage target = new FilesystemMusicStorage(fileSystemMock, "RootDir", false);

			//	Act

			target.AddSongAsync(@"SomeSourceDir\SourceName.mp3", new Uri("/Foreign/SomeAlbum/DestinationName.mp3", UriKind.Relative)).Wait();

			//	Assert

			fileSystemMock.Received(1).SetReadOnlyAttribute(@"RootDir\Foreign\SomeAlbum\DestinationName.mp3");
		}
	}
}
