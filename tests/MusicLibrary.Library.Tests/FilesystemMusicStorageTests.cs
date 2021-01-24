using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.Library.Tests
{
	[TestFixture]
	public class FilesystemMusicStorageTests
	{
		[Test]
		public void StoreSong_StoresSongFileCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			target.StoreSong("SourceSongFile.mp3", song).Wait();

			// Assert

			fileStorageMock.Received(1).StoreFile("SourceSongFile.mp3", new Uri("/SomeSongName.mp3", UriKind.Relative));
		}

		[Test]
		public void GetSongFile_ReturnsSongFileFromStorage()
		{
			// Arrange

			IFileStorage fileStorageStub = Substitute.For<IFileStorage>();
			fileStorageStub.GetFile(new Uri("/SomeSongName.mp3", UriKind.Relative)).Returns("SomeSongFile.mp3");
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageStub);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			var songFile = target.GetSongFile(song).Result;

			// Assert

			Assert.AreEqual("SomeSongFile.mp3", songFile);
		}

		[Test]
		public void GetSongFileForWriting_ReturnsFileForWritingFromStorage()
		{
			// Arrange

			IFileStorage fileStorageStub = Substitute.For<IFileStorage>();
			fileStorageStub.GetFileForWriting(new Uri("/SomeSongName.mp3", UriKind.Relative)).Returns("SomeSongFileForWriting.mp3");
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageStub);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			var songFile = target.GetSongFileForWriting(song).Result;

			// Assert

			Assert.AreEqual("SomeSongFileForWriting.mp3", songFile);
		}

		[Test]
		public void UpdateSongContent_UpdatesSongFileContentCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			target.UpdateSongContent("SourceSongFile.mp3", song).Wait();

			// Assert

			fileStorageMock.Received(1).UpdateFileContent("SourceSongFile.mp3", new Uri("/SomeSongName.mp3", UriKind.Relative));
		}

		[Test]
		public void ChangeSongUri_MovesSongFileCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			target.ChangeSongUri(song, new Uri("/NewSongName.mp3", UriKind.Relative)).Wait();

			// Assert

			fileStorageMock.Received(1).MoveFile(new Uri("/SomeSongName.mp3", UriKind.Relative), new Uri("/NewSongName.mp3", UriKind.Relative));
		}

		[Test]
		public void DeleteSong_DeletesSongFileCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var song = new Song
			{
				Uri = new Uri("/SomeSongName.mp3", UriKind.Relative),
			};

			// Act

			target.DeleteSong(song).Wait();

			// Assert

			fileStorageMock.Received(1).DeleteFile(new Uri("/SomeSongName.mp3", UriKind.Relative));
		}

		[Test]
		public void ChangeDiscUri_MovesDiscDirectoryCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative),
			};

			// Act

			target.ChangeDiscUri(disc, new Uri("/NewDiscUri", UriKind.Relative)).Wait();

			// Assert

			fileStorageMock.Received(1).MoveDirectory(new Uri("/SomeDisc", UriKind.Relative), new Uri("/NewDiscUri", UriKind.Relative));
		}

		[Test]
		public void StoreDiscImage_StoresDiscImageFileCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var discImage = new DiscImage
			{
				Uri = new Uri("/SomeDiscImage", UriKind.Relative),
			};

			// Act

			target.StoreDiscImage("SomeImageFile.img", discImage).Wait();

			// Assert

			fileStorageMock.Received(1).StoreFile("SomeImageFile.img", new Uri("/SomeDiscImage", UriKind.Relative));
		}

		[Test]
		public void GetDiscImageFile_ReturnsDiscImageFileFromStorage()
		{
			// Arrange

			IFileStorage fileStorageStub = Substitute.For<IFileStorage>();
			fileStorageStub.GetFile(new Uri("/SomeDiscImage", UriKind.Relative)).Returns("SomeImageFile.img");
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageStub);

			var discImage = new DiscImage
			{
				Uri = new Uri("/SomeDiscImage", UriKind.Relative),
			};

			// Act

			var imageFile = target.GetDiscImageFile(discImage).Result;

			// Assert

			Assert.AreEqual("SomeImageFile.img", imageFile);
		}

		[Test]
		public void DeleteDiscImage_DeletesDiscImageFileCorrectly()
		{
			// Arrange

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			var discImage = new DiscImage
			{
				Uri = new Uri("/SomeDiscImage", UriKind.Relative),
			};

			// Act

			target.DeleteDiscImage(discImage).Wait();

			// Assert

			fileStorageMock.Received(1).DeleteFile(new Uri("/SomeDiscImage", UriKind.Relative));
		}

		[Test]
		public void CheckDataConsistency_ChecksStorageConsistencyExcludingSyncDirectory()
		{
			// Arrange

			var expectedItemUris = Array.Empty<Uri>();
			List<Uri> passedIgnoreList = null;

			ILibraryStorageInconsistencyRegistrator registrator = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			var checkScopeStub = Substitute.For<IUriCheckScope>();

			IFileStorage fileStorageMock = Substitute.For<IFileStorage>();
			fileStorageMock.CheckDataConsistency(Arg.Any<IEnumerable<Uri>>(), Arg.Do<IEnumerable<Uri>>(ignoreList => passedIgnoreList = ignoreList.ToList()),
				checkScopeStub, registrator, Arg.Any<bool>());

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileStorageMock);

			// Act

			target.CheckDataConsistency(expectedItemUris, checkScopeStub, registrator, false).Wait();

			// Assert

			fileStorageMock.Received(1).CheckDataConsistency(expectedItemUris, Arg.Any<IEnumerable<Uri>>(), checkScopeStub, registrator, false);
			Assert.AreEqual(new[] { new Uri("/.sync", UriKind.Relative) }, passedIgnoreList);
		}
	}
}
