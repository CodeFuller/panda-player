using System;
using System.Text;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.PandaPlayer.Tests
{
	[TestFixture]
	public class JsonFileGenericRepositoryTests
	{
		[Test]
		public void Save_SavesDataToTargetDataFile()
		{
			// Arrange

			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, Substitute.For<ILogger<JsonFileGenericRepository<Object>>>(), "SomeFile.json");

			// Act

			target.Save(new Object());

			// Assert

			fileSystemFacadeMock.Received(1).WriteAllText("SomeFile.json", Arg.Any<string>(), Encoding.UTF8);
		}

		[Test]
		public void Load_IfDataFileDoesNotExist_ReturnsNull()
		{
			// Arrange

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeStub, Substitute.For<ILogger<JsonFileGenericRepository<Object>>>(), "SomeFile.json");

			// Act

			var data = target.Load();

			// Assert

			Assert.IsNull(data);
		}

		[Test]
		public void Load_IfDataFileExists_LoadsDataFromDataFile()
		{
			// Arrange

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(true);
			fileSystemFacadeStub.ReadAllText("SomeFile.json", Encoding.UTF8).Returns("{\"SomeProperty\": \"SomeValue\"}");
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeStub, Substitute.For<ILogger<JsonFileGenericRepository<Object>>>(), "SomeFile.json");

			// Act

			var data = target.Load();

			// Assert

			Assert.IsNotNull(data);
		}

		[Test]
		public void Purge_IfDataFileDoesNotExist_DoesNothing()
		{
			// Arrange

			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(false);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, Substitute.For<ILogger<JsonFileGenericRepository<Object>>>(), "SomeFile.json");

			// Act

			target.Purge();

			// Assert

			fileSystemFacadeMock.DidNotReceive().DeleteFile(Arg.Any<string>());
		}

		[Test]
		public void Purge_IfDataFileExists_DeletesDataFile()
		{
			// Arrange

			IFileSystemFacade fileSystemFacadeMock = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeMock.FileExists("SomeFile.json").Returns(true);
			var target = new JsonFileGenericRepository<Object>(fileSystemFacadeMock, Substitute.For<ILogger<JsonFileGenericRepository<Object>>>(), "SomeFile.json");

			// Act

			target.Purge();

			// Assert

			fileSystemFacadeMock.Received(1).DeleteFile("SomeFile.json");
		}
	}
}
