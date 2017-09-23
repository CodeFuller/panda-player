using System;
using System.Text;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using NSubstitute;
using NUnit.Framework;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	[TestFixture]
	public class JsonFilePlaylistDataRepositoryTests
	{
		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonFilePlaylistDataRepository(null, "SomeFile.json"));
		}

		[Test]
		public void Constructor_IfDataFileNameArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonFilePlaylistDataRepository(Substitute.For<IFileSystemFacade>(), null));
		}

		[Test]
		public void Save_SavesDataToTargetDataFile()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			var target = new JsonFilePlaylistDataRepository(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Save(new PlaylistData());

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllText("SomeFile.json", Arg.Any<string>(), Encoding.UTF8);
		}

		[Test]
		public void Load_IfDataFileDoesNotExist_ReturnsNull()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFilePlaylistDataRepository(fileSystemFacadeStub, "SomeFile.json");

			//	Act

			var playlistData = target.Load();

			//	Assert

			Assert.IsNull(playlistData);
		}

		[Test]
		public void Load_IfDataFileExists_ReadsPlaylistDataFromDataFile()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(true);
			fileSystemFacadeStub.ReadAllText("SomeFile.json", Encoding.UTF8).Returns("{\"Songs\": null, \"CurrentSong\": null}");
			var target = new JsonFilePlaylistDataRepository(fileSystemFacadeStub, "SomeFile.json");

			//	Act

			var playlistData = target.Load();

			//	Assert

			Assert.IsNotNull(playlistData);
		}

		[Test]
		public void Purge_IfDataFileDoesNotExist_DoesNothing()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFilePlaylistDataRepository(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Purge();

			//	Assert

			fileSystemFacadeMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void Purge_IfDataFileExists_DeletesDataFile()
		{
			//	Arrange

			Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(true);
			var target = new JsonFilePlaylistDataRepository(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Purge();

			//	Assert

			fileSystemFacadeMock.Received(1).DeleteFile("SomeFile.json");
		}
	}
}
