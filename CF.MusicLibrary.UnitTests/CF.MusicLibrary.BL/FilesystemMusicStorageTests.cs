using System;
using CF.Library.Core.Facades;
using CF.MusicLibrary.BL;
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
		public void AddSongAsync_CreatesDestinationDirectory()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), "RootDir");

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
		public void AddSongAsync_CopiesSongFileCorrectly()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), "RootDir");

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
		public void AddSongAsync_BeforeTaggingSongFile_ClearsReadOnlyAttribute()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			ISongTagger songTaggerStub = Substitute.For<ISongTagger>();

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, songTaggerStub, "RootDir");

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
		public void AddSongAsync_SetsReadOnlyAttributeForDestinationSongFile()
		{
			//	Arrange

			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();
			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemMock, Substitute.For<ISongTagger>(), "RootDir");

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
		public void AddSongAsync_SetsCorrectTagData()
		{
			//	Arrange

			SongTagData setTagData = null;

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			songTaggerMock.SetTagData(Arg.Any<string>(), Arg.Do<SongTagData>(arg => setTagData = arg));

			FileSystemMusicStorage target = new FileSystemMusicStorage(fileSystemStub, songTaggerMock, "RootDir");

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
	}
}
