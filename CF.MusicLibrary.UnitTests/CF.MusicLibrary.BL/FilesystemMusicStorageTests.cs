using System;
using System.Linq;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.BL
{
	[TestFixture]
	public class FilesystemMusicStorageTests
	{
		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new FileSystemMusicStorage(null, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir"));
		}

		[Test]
		public void Constructor_IfSongTaggerArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), null, Substitute.For<IDiscArtFileStorage>(), "RootDir"));
		}

		[Test]
		public void Constructor_IfDiscArtFileStorageArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), Substitute.For<ISongTagger>(), null, "RootDir"));
		}

		[Test]
		public void Constructor_IfLibraryRootDirectoryArgumentIsNull_ThrowsArgumentIsNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), null));
		}

		[Test]
		public void AddSong_CreatesDestinationDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
				Uri = new Uri("/Foreign/SomeDisc/DestinationName.mp3", UriKind.Relative),
			};

			//	Act

			target.AddSong(song, @"SomeSourceDir\SourceName.mp3").Wait();

			//	Assert

			fileSystemMock.Received(1).CreateDirectory(@"RootDir\Foreign\SomeDisc");
		}

		[Test]
		public void AddSong_CopiesSongFileCorrectly()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
				Uri = new Uri("/Foreign/SomeDisc/DestinationName.mp3", UriKind.Relative),
			};

			//	Act

			target.AddSong(song, @"SomeSourceDir\SourceName.mp3").Wait();

			//	Assert

			fileSystemMock.Received(1).CopyFile(@"SomeSourceDir\SourceName.mp3", @"RootDir\Foreign\SomeDisc\DestinationName.mp3");
		}

		[Test]
		public void AddSong_BeforeTaggingSongFile_ClearsReadOnlyAttribute()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			ISongTagger songTaggerStub = Substitute.For<ISongTagger>();

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, songTaggerStub, Substitute.For<IDiscArtFileStorage>(), "RootDir");

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
				Uri = new Uri("/Foreign/SomeDisc/DestinationName.mp3", UriKind.Relative),
			};

			//	Act

			target.AddSong(song, @"SomeSourceDir\SourceName.mp3").Wait();

			//	Assert

			Received.InOrder(() =>
			{
				fileSystemMock.ClearReadOnlyAttribute(@"RootDir\Foreign\SomeDisc\DestinationName.mp3");
				songTaggerStub.SetTagData(@"RootDir\Foreign\SomeDisc\DestinationName.mp3", Arg.Any<SongTagData>());
			});
		}

		[Test]
		public void AddSong_SetsReadOnlyAttributeForDestinationSongFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
				Uri = new Uri("/Foreign/SomeDisc/DestinationName.mp3", UriKind.Relative),
			};

			//	Act

			target.AddSong(song, @"SomeSourceDir\SourceName.mp3").Wait();

			//	Assert

			fileSystemMock.Received(1).SetReadOnlyAttribute(@"RootDir\Foreign\SomeDisc\DestinationName.mp3");
		}

		[Test]
		public void AddSong_SetsCorrectTagData()
		{
			//	Arrange

			SongTagData setTagData = null;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			songTaggerMock.SetTagData(Arg.Any<string>(), Arg.Do<SongTagData>(arg => setTagData = arg));

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemStub, songTaggerMock, Substitute.For<IDiscArtFileStorage>(), "RootDir");

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
				Uri = new Uri("/Foreign/SomeDisc/DestinationName.mp3", UriKind.Relative),
			};

			//	Act

			target.AddSong(song, @"SomeSourceDir\SourceName.mp3").Wait();

			//	Assert

			songTaggerMock.Received(1).SetTagData(@"RootDir\Foreign\SomeDisc\DestinationName.mp3", Arg.Any<SongTagData>());
			Assert.AreEqual(song.Artist.Name, setTagData.Artist);
			Assert.AreEqual(song.Disc.AlbumTitle, setTagData.Album);
			Assert.AreEqual(song.Year, setTagData.Year);
			Assert.AreEqual(song.Genre.Name, setTagData.Genre);
			Assert.AreEqual(song.TrackNumber, setTagData.Track);
			Assert.AreEqual(song.Title, setTagData.Title);
		}

		[Test]
		public void DeleteSong_DeletesSongFileCorrectly()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			Received.InOrder(() =>
			{
				fileSystemMock.ClearReadOnlyAttribute(@"RootDir\SomeDisc\SomeSong.mp3");
				fileSystemMock.DeleteFile(@"RootDir\SomeDisc\SomeSong.mp3");
			});
		}

		[Test]
		public void DeleteSong_IfDiscDirectoryContainsSomeOtherSongFiles_DoesNotDeleteOtherContent()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateFiles(@"RootDir\SomeDisc").Returns(new[] { @"RootDir\SomeDisc\AnotherSong1.mp3", @"RootDir\SomeDisc\AnotherSong2.mp3" });

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteFile(Arg.Any<string>());
			fileSystemMock.DidNotReceive().DeleteDirectory(Arg.Any<string>());
		}

		[Test]
		public void DeleteSong_IfDiscDirectoryContainsOneMoreFileWhichIsNotCoverImage_DoesNotDeleteOtherContent()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateFiles(@"RootDir\SomeDisc").Returns(new[] { @"RootDir\SomeDisc\AnotherSong.mp3" });

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.IsCoverImageFile(@"RootDir\SomeDisc\AnotherSong.mp3").Returns(false);

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), discArtFileStorageStub, "RootDir");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteFile(Arg.Any<string>());
			fileSystemMock.DidNotReceive().DeleteDirectory(Arg.Any<string>());
		}

		[Test]
		public void DeleteSong_IfRestDiscDirectoryContainsOnlyCoverImage_DeletesCoverImageFile()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateFiles(@"RootDir\SomeDisc").Returns(new[] { @"RootDir\SomeDisc\cover.jpg" });

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.IsCoverImageFile(@"RootDir\SomeDisc\cover.jpg").Returns(true);

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), discArtFileStorageStub, "RootDir");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteFile(@"RootDir\SomeDisc\cover.jpg");
		}

		[Test]
		public void DeleteSong_IfDiscDirectoryBecomeEmpty_DeletesDiscDirectory()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateFiles(@"RootDir\SomeDisc").Returns(Enumerable.Empty<string>());
			fileSystemMock.DirectoryIsEmpty(@"RootDir\SomeDisc").Returns(true);

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), "RootDir");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory(@"RootDir\SomeDisc");
		}

		[Test]
		public void DeleteSong_IfDiscDirectoryBecomeEmpty_DeletesAllEmptyParentDirectories()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/Folder1/Folder2/Folder3/Folder4/SomeDisc/SomeSong.mp3", UriKind.Relative)
			};

			Application.Logger = Substitute.For<IMessageLogger>();

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			fileSystemMock.EnumerateFiles(@"c:\temp\music\Folder1\Folder2\Folder3\Folder4\SomeDisc").Returns(Enumerable.Empty<string>());
			fileSystemMock.DirectoryIsEmpty(@"c:\temp\music\Folder1\Folder2\Folder3\Folder4\SomeDisc").Returns(true);
			fileSystemMock.DirectoryIsEmpty(@"c:\temp\music\Folder1\Folder2\Folder3\Folder4").Returns(true);
			fileSystemMock.DirectoryIsEmpty(@"c:\temp\music\Folder1\Folder2\Folder3").Returns(true);
			fileSystemMock.DirectoryIsEmpty(@"c:\temp\music\Folder1\Folder2").Returns(false);

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), Substitute.For<IDiscArtFileStorage>(), @"c:\temp\music");

			//	Act

			target.DeleteSong(song).Wait();

			//	Assert

			fileSystemMock.Received(1).DeleteDirectory(@"c:\temp\music\Folder1\Folder2\Folder3\Folder4");
			fileSystemMock.Received(1).DeleteDirectory(@"c:\temp\music\Folder1\Folder2\Folder3");
			fileSystemMock.DidNotReceive().DeleteDirectory(@"c:\temp\music\Folder1\Folder2");
		}

		[Test]
		public void SetDiscCoverImage_CallsDiscArtFileStorageCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative),
			};

			IDiscArtFileStorage discArtFileStorageMock = Substitute.For<IDiscArtFileStorage>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), Substitute.For<ISongTagger>(), discArtFileStorageMock, "RootDir");

			//	Act

			target.SetDiscCoverImage(disc, "SomeNewCover.tmp").Wait();

			//	Assert

			discArtFileStorageMock.Received(1).StoreDiscCoverImage(@"RootDir\SomeDisc", "SomeNewCover.tmp");
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasCoverImage_ReturnsDiscCoverCorrectly()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative),
			};

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"RootDir\SomeDisc").Returns(@"RootDir\SomeDisc\SomeCover.img");
			FileSystemMusicStorage target = new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), Substitute.For<ISongTagger>(), discArtFileStorageStub, "RootDir");

			//	Act

			var imageFileName = target.GetDiscCoverImage(disc).Result;

			//	Assert

			Assert.AreEqual(@"RootDir\SomeDisc\SomeCover.img", imageFileName);
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasNoCoverImage_ReturnsNull()
		{
			//	Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative),
			};

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"RootDir\SomeDisc").Returns((string)null);
			FileSystemMusicStorage target = new FileSystemMusicStorage(Substitute.For<IFileSystemFacade>(), Substitute.For<ISongTagger>(), discArtFileStorageStub, "RootDir");

			//	Act

			var imageFileName = target.GetDiscCoverImage(disc).Result;

			//	Assert

			Assert.IsNull(imageFileName);
		}
	}
}
