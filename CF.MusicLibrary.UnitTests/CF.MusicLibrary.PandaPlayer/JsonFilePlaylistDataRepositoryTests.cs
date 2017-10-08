using System;
using System.Text;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.MusicLibrary.PandaPlayer;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.PandaPlayer
{
	[TestFixture]
	public class JsonFileGenericRepositoryTests
	{
		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonFileGenericRepository<Object>(null, "SomeFile.json"));
		}

		[Test]
		public void Constructor_IfDataFileNameArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonFileGenericRepository<Object>(Substitute.For<IFileSystemFacade>(), null));
		}

		[Test]
		public void Save_SavesDataToTargetDataFile()
		{
			//	Arrange

			Application.Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Save(new Object());

			//	Assert

			fileSystemFacadeMock.Received(1).WriteAllText("SomeFile.json", Arg.Any<string>(), Encoding.UTF8);
		}

		[Test]
		public void Load_IfDataFileDoesNotExist_ReturnsNull()
		{
			//	Arrange

			Application.Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeStub, "SomeFile.json");

			//	Act

			var data = target.Load();

			//	Assert

			Assert.IsNull(data);
		}

		[Test]
		public void Load_IfDataFileExists_LoadsDataFromDataFile()
		{
			//	Arrange

			Application.Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(true);
			fileSystemFacadeStub.ReadAllText("SomeFile.json", Encoding.UTF8).Returns("{\"SomeProperty\": \"SomeValue\"}");
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeStub, "SomeFile.json");

			//	Act

			var data = target.Load();

			//	Assert

			Assert.IsNotNull(data);
		}

		[Test]
		public void Purge_IfDataFileDoesNotExist_DoesNothing()
		{
			//	Arrange

			Application.Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Purge();

			//	Assert

			fileSystemFacadeMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void Purge_IfDataFileExists_DeletesDataFile()
		{
			//	Arrange

			Application.Logger = Substitute.For<IMessageLogger>();
			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(true);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, "SomeFile.json");

			//	Act

			target.Purge();

			//	Assert

			fileSystemFacadeMock.Received(1).DeleteFile("SomeFile.json");
		}
	}
}
