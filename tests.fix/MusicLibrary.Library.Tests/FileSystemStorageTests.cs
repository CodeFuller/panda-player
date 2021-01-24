using System;
using System.IO;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.Library.Tests
{
	[TestFixture]
	public class FileSystemStorageTests
	{
		[Test]
		public void StoreFile_BeforeStoringFile_CreatesDestinationDirectory()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.StoreFile("SourceFileName", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			Received.InOrder(() =>
			{
				fileSystemMock.Received(1).CreateDirectory(@"RootDir\SomeDir");
				fileSystemMock.Received(1).CopyFile(Arg.Any<string>(), Arg.Any<string>());
			});
		}

		[Test]
		public void StoreFile_CopiesFileCorrectly()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.StoreFile("SourceFileName", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).CopyFile("SourceFileName", @"RootDir\SomeDir\SomeFile");
		}

		[Test]
		public void StoreFile_SetsReadOnlyAttributeForDestinationFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.StoreFile("SourceFileName", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).SetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile");
		}

		[Test]
		public void GetFile_IfFileExists_ReturnsCorrectPathToStorageFile()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			// Act

			var fileName = target.GetFile(new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			Assert.AreEqual(@"RootDir\SomeDir\SomeFile", fileName);
		}

		[Test]
		public void GetFile_IfFileDoesNotExist_ThrowsInvalidOperationException()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.GetFile(new Uri("/SomeDir/SomeFile", UriKind.Relative)));
		}

		[Test]
		public void GetFileForWriting_IfFileExists_ReturnsCorrectPathToStorageFile()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			// Act

			var fileName = target.GetFileForWriting(new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			Assert.AreEqual(@"RootDir\SomeDir\SomeFile", fileName);
		}

		[Test]
		public void GetFileForWriting_IfFileExists_ClearsReadOnlyAttributeForFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.GetFileForWriting(new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).ClearReadOnlyAttribute(@"RootDir\SomeDir\SomeFile");
		}

		[Test]
		public void GetFileForWriting_IfFileDoesNotExist_ThrowsInvalidOperationException()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.GetFileForWriting(new Uri("/SomeDir/SomeFile", UriKind.Relative)));
		}

		[Test]
		public void UpdateFileContent_IfDestinationFileDoesNotExist_ThrowsInvalidOperationException()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.UpdateFileContent(@"RootDir\SomeDir\SomeFile", new Uri("/SomeDir/SomeFile", UriKind.Relative)));
		}

		[Test]
		public void UpdateFileContent_IfSourceAndDestinationFilePathsAreEqual_JustSetsReadOnlyAttributeForFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.UpdateFileContent(@"RootDir\SomeDir\SomeFile", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).SetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile");
			fileSystemMock.DidNotReceive().CopyFile(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void UpdateFileContent_IfSourceAndDestinationFilePathsDiffer_DeletesExistingFileBeforeCopyingNew()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.UpdateFileContent("SomeSourceFile", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			Received.InOrder(() =>
			{
				fileSystemMock.Received(1).ClearReadOnlyAttribute(@"RootDir\SomeDir\SomeFile");
				fileSystemMock.Received(1).DeleteFile(@"RootDir\SomeDir\SomeFile");
				fileSystemMock.Received(1).CopyFile(Arg.Any<string>(), Arg.Any<string>());
			});
		}

		[Test]
		public void UpdateFileContent_IfSourceAndDestinationFilePathsDiffer_StoresNewFileCorrectly()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.UpdateFileContent("SomeSourceFile", new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).CopyFile("SomeSourceFile", @"RootDir\SomeDir\SomeFile");
		}

		[Test]
		public void MoveFile_MovesFileCorrectly()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.MoveFile(new Uri("/SomeDir1/SomeOldName", UriKind.Relative), new Uri("/SomeDir2/SomeNewName", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).MoveFile(@"RootDir\SomeDir1\SomeOldName", @"RootDir\SomeDir2\SomeNewName");
		}

		[Test]
		public void MoveFile_MovesDirectoryCorrectly()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.MoveDirectory(new Uri("/SomeDir/SubDir1", UriKind.Relative), new Uri("/SomeDir/SubDir2", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).MoveDirectory(@"RootDir\SomeDir\SubDir1", @"RootDir\SomeDir\SubDir2");
		}

		[Test]
		public void DeleteFile_IfFileDirectoryContainsOtherFiles_DoesNotDeleteFileDirectory()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));
			fileSystemMock.DirectoryIsEmpty(@"RootDir\SomeDir").Returns(false);

			// Act

			target.DeleteFile(new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.DidNotReceive().DeleteDirectory(Arg.Any<string>());
		}

		[Test]
		public void DeleteFile_IfFileDirectoryBecomesEmpty_DeletesFileDirectory()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.GetFullPath(Arg.Any<string>()).Returns(x => (string)x[0]);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));
			fileSystemMock.DirectoryIsEmpty(@"RootDir\SomeDir").Returns(true);
			fileSystemMock.GetParentDirectory(@"RootDir\SomeDir\SomeFile").Returns(@"RootDir\SomeDir");

			// Act

			target.DeleteFile(new Uri("/SomeDir/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).DeleteDirectory(@"RootDir\SomeDir");
		}

		[Test]
		public void DeleteFile_IfFileDirectoryBecomesEmpty_DeletesAllEmptyParentDirectories()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.GetFullPath(Arg.Any<string>()).Returns(x => (string)x[0]);
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2\Folder3\Folder4\SomeFile").Returns(@"RootDir\Folder1\Folder2\Folder3\Folder4");
			fileSystemMock.DirectoryIsEmpty(@"RootDir\Folder1\Folder2\Folder3\Folder4").Returns(true);
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2\Folder3\Folder4").Returns(@"RootDir\Folder1\Folder2\Folder3");
			fileSystemMock.DirectoryIsEmpty(@"RootDir\Folder1\Folder2\Folder3").Returns(true);
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2\Folder3").Returns(@"RootDir\Folder1\Folder2");
			fileSystemMock.DirectoryIsEmpty(@"RootDir\Folder1\Folder2").Returns(false);
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2").Returns(@"RootDir\Folder1");
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2").Returns(@"RootDir");
			fileSystemMock.GetParentDirectory(@"RootDir\Folder1\Folder2").Returns((string)null);

			var settings = new FileSystemStorageSettings { Root = "RootDir" };

			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.DeleteFile(new Uri("/Folder1/Folder2/Folder3/Folder4/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).DeleteDirectory(@"RootDir\Folder1\Folder2\Folder3\Folder4");
			fileSystemMock.Received(1).DeleteDirectory(@"RootDir\Folder1\Folder2\Folder3");
			fileSystemMock.Received(2).DeleteDirectory(Arg.Any<string>());
		}

		[Test]
		public void DeleteFile_WhenDeletingEmptyDirectories_DoesNotGoOutsideLibraryRoot()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.GetFullPath(Arg.Any<string>()).Returns(x => (string)x[0]);
			fileSystemMock.GetParentDirectory(@"SomeOuterDir\RootDir\SubFolder\SomeFile").Returns(@"SomeOuterDir\RootDir\SubFolder");
			fileSystemMock.GetParentDirectory(@"SomeOuterDir\RootDir\SubFolder").Returns(@"SomeOuterDir\RootDir");
			fileSystemMock.GetParentDirectory(@"SomeOuterDir\RootDir").Returns(@"SomeOuterDir");
			fileSystemMock.GetParentDirectory(@"SomeOuterDir").Returns((string)null);
			fileSystemMock.DirectoryIsEmpty(Arg.Any<string>()).Returns(true);

			var settings = new FileSystemStorageSettings { Root = @"SomeOuterDir\RootDir" };

			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			// Act

			target.DeleteFile(new Uri(@"/SubFolder/SomeFile", UriKind.Relative));

			// Assert

			fileSystemMock.Received(1).DeleteDirectory(@"SomeOuterDir\RootDir\SubFolder");
			fileSystemMock.Received(1).DeleteDirectory(Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileDoesNotExist_RegistersStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.Received(1).RegisterMissingStorageData(new Uri("/SomeDir/SomeFile", UriKind.Relative));
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileExists_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterMissingStorageData(Arg.Any<Uri>());
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileHasNoReadOnlyAttributeSet_RegistersStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			fileSystemStub.GetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile").Returns(false);

			var settings = new FileSystemStorageSettings { Root = "RootDir" };

			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.Received(1).RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileHasReadOnlyAttributeSet_DoesNotRegistersStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			fileSystemStub.GetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile").Returns(true);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileHasNoReadOnlyAttributeSetAndFixFoundIssuesIsTrue_SetsReadOnlyAttributeForFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			fileSystemMock.GetReadOnlyAttribute(@"RootDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(),
				checkScopeStub, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), true);

			// Assert

			fileSystemMock.SetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile");
		}

		[Test]
		public void CheckDataConsistency_IfExpectedFileHasNoReadOnlyAttributeSetAndFixFoundIssuesIsFalse_DoesNotSetReadOnlyAttributeForFile()
		{
			// Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.FileExists(@"RootDir\SomeDir\SomeFile").Returns(true);
			fileSystemMock.GetReadOnlyAttribute(@"RootDir\SomeDir\SomeFile").Returns(false);
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemMock, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(),
				checkScopeStub, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false);

			// Assert

			fileSystemMock.DidNotReceive().SetReadOnlyAttribute(Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_ForExpectedStorageFiles_DoesNotRegistersUnexpectedStorageDataInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateFiles(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir\SomeFile" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_IfUnexpectedFileExistsNotListedInIgnoreList_RegistersStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateFiles(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir\SomeFile" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.Received(1).RegisterUnexpectedStorageData(@"RootDir\SomeDir\SomeFile", "file");
		}

		[Test]
		public void CheckDataConsistency_IfUnexpectedFileExistsListedInIgnoreList_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateFiles(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir\SomeFile" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), new[] { new Uri("/SomeDir", UriKind.Relative) }, checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_ForUnexpectedFilesOutsideCheckScope_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateFiles(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir\SomeFile" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(new Uri("/SomeDir/SomeFile", UriKind.Relative)).Returns(false);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_ForExpectedStorageDirectories_DoesNotRegistersUnexpectedStorageDataInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateDirectories(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(new[] { new Uri("/SomeDir/SomeFile", UriKind.Relative) }, Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_IfUnexpectedDirectoryExists_RegistersStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateDirectories(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.Received(1).RegisterUnexpectedStorageData(@"RootDir\SomeDir", "directory");
		}

		[Test]
		public void CheckDataConsistency_IfUnexpectedDirectoryExistsListedInIgnoreList_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateDirectories(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir\SubDir" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Uri>()).Returns(true);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), new[] { new Uri("/SomeDir", UriKind.Relative) }, checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}

		[Test]
		public void CheckDataConsistency_ForUnexpectedDirectoriesOutsideCheckScope_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.EnumerateDirectories(@"RootDir", "*.*", SearchOption.AllDirectories).Returns(new[] { @"RootDir\SomeDir" });
			var settings = new FileSystemStorageSettings { Root = "RootDir" };
			var target = new FileSystemStorage(fileSystemStub, Options.Create(settings));

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(new Uri("/SomeDir", UriKind.Relative)).Returns(false);

			var registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckDataConsistency(Array.Empty<Uri>(), Array.Empty<Uri>(), checkScopeStub, registratorMock, false);

			// Assert

			registratorMock.DidNotReceive().RegisterUnexpectedStorageData(Arg.Any<string>(), Arg.Any<string>());
		}
	}
}
