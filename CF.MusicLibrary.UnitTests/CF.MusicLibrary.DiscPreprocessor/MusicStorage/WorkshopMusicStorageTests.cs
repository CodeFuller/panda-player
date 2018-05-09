using System;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.DiscPreprocessor;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.MusicStorage
{
	[TestFixture]
	public class WorkshopMusicStorageTests
	{
		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new WorkshopMusicStorage(null, new DiscPreprocessorSettings().StubOptions()));
		}

		[Test]
		public void DeleteSourceContent_DeletesContentFilesCorrectly()
		{
			//	Arrange

			var contentFiles = new[]
			{
				"Content File 1.mp3",
				"Content File 2.jpg",
			};

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new DiscPreprocessorSettings { WorkshopStoragePath = "SomeStorage" };
			var target = new WorkshopMusicStorage(fileSystemMock, settings.StubOptions());

			//	Act

			target.DeleteSourceContent(contentFiles);

			//	Assert

			Received.InOrder(() =>
			{
				fileSystemMock.ClearReadOnlyAttribute("Content File 1.mp3");
				fileSystemMock.DeleteFile("Content File 1.mp3");
				fileSystemMock.ClearReadOnlyAttribute("Content File 2.jpg");
				fileSystemMock.DeleteFile("Content File 2.jpg");
			});
		}

		[Test]
		public void DeleteSourceContent_IfSubDirectoryContainsNoFiles_DeletesSubDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeStorage").Returns(new[] { "SubDirectory1" });
			fileSystemMock.EnumerateFiles("SubDirectory1").Returns(Enumerable.Empty<string>());
			fileSystemMock.EnumerateDirectories("SubDirectory1").Returns(new[] { @"SubDirectory1\SubDirectory11" });

			var settings = new DiscPreprocessorSettings { WorkshopStoragePath = "SomeStorage" };

			var target = new WorkshopMusicStorage(fileSystemMock, settings.StubOptions());

			//	Act

			target.DeleteSourceContent(Enumerable.Empty<string>());

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory("SubDirectory1", true);
		}

		[Test]
		public void DeleteSourceContent_IfSubDirectoryContainsSomeFiles_DoesNotDeleteSubDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeStorage").Returns(new[] { "SubDirectory1" });
			fileSystemMock.EnumerateFiles("SubDirectory1").Returns(new[] { "SomeFile" });

			var settings = new DiscPreprocessorSettings { WorkshopStoragePath = "SomeStorage" };

			var target = new WorkshopMusicStorage(fileSystemMock, settings.StubOptions());

			//	Act

			target.DeleteSourceContent(Enumerable.Empty<string>());

			//	Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void DeleteSourceContent_IfSubDirectoryContainsSomeFilesOnDeepLevels_DoesNotDeleteSubDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateDirectories("SomeStorage").Returns(new[] { "SubDirectory1" });
			fileSystemMock.EnumerateFiles("SubDirectory1").Returns(Enumerable.Empty<string>());
			fileSystemMock.EnumerateDirectories("SubDirectory1").Returns(new[] { @"SubDirectory1\SubDirectory11" });
			fileSystemMock.EnumerateFiles(@"SubDirectory1\SubDirectory11").Returns(new[] { "SomeFile" });

			var settings = new DiscPreprocessorSettings { WorkshopStoragePath = "SomeStorage" };

			var target = new WorkshopMusicStorage(fileSystemMock, settings.StubOptions());

			//	Act

			target.DeleteSourceContent(Enumerable.Empty<string>());

			//	Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(Arg.Any<string>(), Arg.Any<bool>());
		}
	}
}
