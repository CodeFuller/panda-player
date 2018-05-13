using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.Library.Tests
{
	[TestFixture]
	public partial class RepositoryAndStorageMusicLibraryTests
	{
		[Test]
		public void AddSong_FillSongTagDataCorrectly()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
			};

			SongTagData setTagData = null;

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			songTaggerMock.SetTagData(Arg.Any<string>(), Arg.Do<SongTagData>(arg => setTagData = arg));

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSong(song, "SomeSongFileName").Wait();

			// Assert

			songTaggerMock.Received(1).SetTagData("SomeSongFileName", Arg.Any<SongTagData>());
			Assert.AreEqual(song.Artist.Name, setTagData.Artist);
			Assert.AreEqual(song.Disc.AlbumTitle, setTagData.Album);
			Assert.AreEqual(song.Year, setTagData.Year);
			Assert.AreEqual(song.Genre.Name, setTagData.Genre);
			Assert.AreEqual(song.TrackNumber, setTagData.Track);
			Assert.AreEqual(song.Title, setTagData.Title);
		}

		[Test]
		public void AddSong_FillsSongChecksum()
		{
			// Arrange

			var song = new Song
			{
				Disc = new Disc(),
				Checksum = null
			};

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFileName").Returns(12345);

			int? savedChecksum = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.AddSong(Arg.Do<Song>(s => savedChecksum = s.Checksum));
			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSong(song, "SomeSongFileName").Wait();

			// Assert

			Assert.AreEqual(12345, song.Checksum);
			Assert.AreEqual(12345, savedChecksum);
		}

		[Test]
		public void AddSong_CalculatesSongChecksumOnlyAfterSongTagDataIsFilled()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			IChecksumCalculator checksumCalculatorMock = Substitute.For<IChecksumCalculator>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				songTaggerMock, Substitute.For<ILibraryStructurer>(), checksumCalculatorMock, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSong(song, "SomeSongFileName").Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.Received(1).SetTagData("SomeSongFileName", Arg.Any<SongTagData>());
				checksumCalculatorMock.Received(1).CalculateChecksumForFile("SomeSongFileName");
			});
		}

		[Test]
		public void AddSong_StoresSongAfterSongTagDataIsFilled()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSong(song, "SomeSongFileName").Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.Received(1).SetTagData("SomeSongFileName", Arg.Any<SongTagData>());
				storageMock.Received(1).StoreSong("SomeSongFileName", song);
			});
		}

		[Test]
		public void AddSong_AddsSongToRepositoryAfterAddingToStorage()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageMock, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSong(song, "SomeSongFileName").Wait();

			// Assert

			Received.InOrder(() =>
			{
				storageMock.Received(1).StoreSong("SomeSongFileName", song);
				repositoryMock.Received(1).AddSong(song);
			});
		}

		[Test]
		public void UpdateSong_IfSomeTaggedPropertiesAreUpdated_UpdatesSongTagData()
		{
			// Arrange

			var song = new Song
			{
				Artist = new Artist { Name = "Some Artist" },
				Disc = new Disc { AlbumTitle = "Some Album" },
				Year = 2017,
				Genre = new Genre { Name = "Some Genre" },
				TrackNumber = 7,
				Title = "Some Song",
			};

			SongTagData setTagData = null;

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			songTaggerMock.SetTagData(Arg.Any<string>(), Arg.Do<SongTagData>(arg => setTagData = arg));

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SongFileName");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.ForceTagUpdate).Wait();

			// Assert

			songTaggerMock.Received(1).SetTagData("SongFileName", Arg.Any<SongTagData>());
			Assert.AreEqual(song.Artist.Name, setTagData.Artist);
			Assert.AreEqual(song.Disc.AlbumTitle, setTagData.Album);
			Assert.AreEqual(song.Year, setTagData.Year);
			Assert.AreEqual(song.Genre.Name, setTagData.Genre);
			Assert.AreEqual(song.TrackNumber, setTagData.Track);
			Assert.AreEqual(song.Title, setTagData.Title);
		}

		[Test]
		public void UpdateSong_IfNoTaggedPropertiesAreUpdated_DoesNotUpdateSongTagData()
		{
			// Arrange

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(new Song(), UpdatedSongProperties.None).Wait();

			// Assert

			songTaggerMock.DidNotReceive().SetTagData(Arg.Any<string>(), Arg.Any<SongTagData>());
		}

		[Test]
		public void UpdateSong_IfSomeTaggedPropertiesAreUpdated_UpdatesSongCheckSum()
		{
			// Arrange

			var song = new Song
			{
				Disc = new Disc(),
				Checksum = 54321,
			};

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SomeSongFileName");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFileName").Returns(12345);

			int? savedChecksum = null;
			IMusicLibraryRepository repository = Substitute.For<IMusicLibraryRepository>();
			repository.UpdateSong(Arg.Do<Song>(s => savedChecksum = s.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repository, storageStub, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.ForceTagUpdate).Wait();

			// Assert

			Assert.AreEqual(12345, song.Checksum);
			Assert.AreEqual(12345, savedChecksum);
		}

		[Test]
		public void UpdateSong_IfSomeTaggedPropertiesAreUpdated_CalculatesSongChecksumOnlyAfterSongTagDataIsUpdated()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SongFileName");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			IChecksumCalculator checksumCalculatorMock = Substitute.For<IChecksumCalculator>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), checksumCalculatorMock, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.ForceTagUpdate).Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.SetTagData("SongFileName", Arg.Any<SongTagData>());
				checksumCalculatorMock.CalculateChecksumForFile("SongFileName");
			});
		}

		[Test]
		public void UpdateSong_IfNoTaggedPropertiesAreUpdated_DoesNotUpdateSongCheckSum()
		{
			// Arrange

			var song = new Song
			{
				Disc = new Disc(),
				Checksum = 54321,
			};

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile(Arg.Any<string>()).Returns(12345);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.None).Wait();

			// Assert

			Assert.AreEqual(54321, song.Checksum);
		}

		[Test]
		public void UpdateSong_IfSomeTaggedPropertiesAreUpdated_UpdateSongContentInStorage()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();
			storageMock.GetSongFileForWriting(song).Returns("SongFileName");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.ForceTagUpdate).Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.Received(1).SetTagData("SongFileName", Arg.Any<SongTagData>());
				storageMock.Received(1).UpdateSongContent("SongFileName", song);
			});
		}

		[Test]
		public void UpdateSong_IfSomeTaggedPropertiesAreUpdated_UpdateSongInRepository()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.ForceTagUpdate).Wait();

			// Assert

			repositoryMock.Received(1).UpdateSong(song);
		}

		[Test]
		public void UpdateSong_IfNoTaggedPropertiesAreUpdated_UpdateSongInRepository()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateSong(song, UpdatedSongProperties.None).Wait();

			// Assert

			repositoryMock.Received(1).UpdateSong(song);
		}

		[Test]
		public void ChangeSongUri_ChangesSongUriInStorage()
		{
			// Arrange

			var song = new Song { Disc = new Disc() };

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeSongUri(song, new Uri("/NewSongUri", UriKind.Relative)).Wait();

			// Assert

			storageMock.Received(1).ChangeSongUri(song, new Uri("/NewSongUri", UriKind.Relative));
		}

		[Test]
		public void ChangeSongUri_SavesNewSongUriInRepository()
		{
			// Arrange

			var song = new Song();

			Uri savedSongUri = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(arg => savedSongUri = arg.Uri));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeSongUri(song, new Uri("/NewSongUri", UriKind.Relative)).Wait();

			// Assert

			Assert.AreEqual(new Uri("/NewSongUri", UriKind.Relative), savedSongUri);
		}

		[Test]
		public void DeleteSong_DeletesSongFromStorage()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.DeleteSong(song, DateTime.Now).Wait();

			// Assert

			storageMock.Received(1).DeleteSong(song);
		}

		[Test]
		public void DeleteSong_SavesSongDeleteTimeInRepository()
		{
			// Arrange

			var song = new Song();

			DateTime? savedDeleteDateTime = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(arg => savedDeleteDateTime = arg.DeleteDate));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.DeleteSong(song, new DateTime(2017, 10, 20, 11, 42, 35)).Wait();

			// Assert

			repositoryMock.Received(1).UpdateSong(song);
			Assert.AreEqual(new DateTime(2017, 10, 20, 11, 42, 35), savedDeleteDateTime);
		}

		[Test]
		public void UpdateDisc_IfSomeTaggedPropertiesAreUpdated_UpdatesSongsTagData()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();
			var disc = new Disc
			{
				SongsUnordered = new List<Song> { song1, song2 },
			};
			song1.Disc = disc;
			song2.Disc = disc;

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song1).Returns("SongFile1");
			storageStub.GetSongFileForWriting(song2).Returns("SongFile2");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(disc, UpdatedSongProperties.Album).Wait();

			// Assert

			songTaggerMock.Received(1).SetTagData("SongFile1", Arg.Any<SongTagData>());
			songTaggerMock.Received(1).SetTagData("SongFile2", Arg.Any<SongTagData>());
		}

		[Test]
		public void UpdateDisc_IfSomeTaggedPropertiesAreUpdated_UpdatesSongsCheckSum()
		{
			// Arrange

			var song = new Song { Checksum = 54321 };
			var disc = new Disc { SongsUnordered = new List<Song> { song } };
			song.Disc = disc;

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SomeSongFileName");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFileName").Returns(12345);

			int? savedChecksum = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(s => savedChecksum = s.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(disc, UpdatedSongProperties.Album).Wait();

			// Assert

			Assert.AreEqual(12345, song.Checksum);
			Assert.AreEqual(12345, savedChecksum);
		}

		[Test]
		public void UpdateDisc_IfSomeTaggedPropertiesAreUpdated_CalculatesSongChecksumOnlyAfterSongTagDataIsUpdated()
		{
			// Arrange

			var song = new Song();
			var disc = new Disc { SongsUnordered = new List<Song> { song } };
			song.Disc = disc;

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SongFileName");
			IChecksumCalculator checksumCalculatorMock = Substitute.For<IChecksumCalculator>();

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), checksumCalculatorMock, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(disc, UpdatedSongProperties.Album).Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.SetTagData("SongFileName", Arg.Any<SongTagData>());
				checksumCalculatorMock.CalculateChecksumForFile("SongFileName");
			});
		}

		[Test]
		public void UpdateDisc_IfNoTaggedPropertiesAreUpdated_DoesNotUpdateSongsTagData()
		{
			// Arrange

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(new Disc { SongsUnordered = new List<Song> { new Song() } }, UpdatedSongProperties.None).Wait();

			// Assert

			songTaggerMock.DidNotReceive().SetTagData(Arg.Any<string>(), Arg.Any<SongTagData>());
		}

		[Test]
		public void UpdateDisc_IfNoTaggedPropertiesAreUpdated_DoesNotUpdateSongsCheckSum()
		{
			// Arrange

			var song = new Song { Checksum = 54321 };
			var disc = new Disc { SongsUnordered = new List<Song> { song } };

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile(Arg.Any<string>()).Returns(12345);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(disc, UpdatedSongProperties.None).Wait();

			// Assert

			Assert.AreEqual(54321, song.Checksum);
		}

		[Test]
		public void UpdateDisc_UpdatesDiscInRepositoryCorrectly()
		{
			// Arrange

			var disc = new Disc();

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.UpdateDisc(disc, UpdatedSongProperties.None).Wait();

			// Assert

			repositoryMock.Received(1).UpdateDisc(disc);
		}

		[Test]
		public void ChangeDiscUri_ChangesDiscUriInStorageCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			Uri oldDiscUri = null;
			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();
			storageMock.ChangeDiscUri(Arg.Do<Disc>(arg => oldDiscUri = arg.Uri), Arg.Any<Uri>());
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			storageMock.Received(1).ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative));
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative), oldDiscUri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesDiscUriPropertyCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), disc.Uri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesDiscInRepositoryCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
			};

			Uri updatedUri = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateDisc(Arg.Do<Disc>(arg => updatedUri = arg.Uri));
			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			repositoryMock.Received(1).UpdateDisc(disc);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), updatedUri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesSongsUriPropertyCorrectly()
		{
			// Arrange

			var song1 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song1", UriKind.Relative) };
			var song2 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				SongsUnordered = new List<Song> { song1, song2, },
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), libraryStructurerStub, Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative), song1.Uri);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative), song2.Uri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesSongsInRepositoryCorrectly()
		{
			// Arrange

			var song1 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song1", UriKind.Relative) };
			var song2 = new Song { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Song2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				SongsUnordered = new List<Song> { song1, song2, },
			};

			List<Uri> updatedUris = new List<Uri>();
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(arg => updatedUris.Add(arg.Uri)));

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInSongUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), song2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), libraryStructurerStub, Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			repositoryMock.Received(1).UpdateSong(song1);
			repositoryMock.Received(1).UpdateSong(song2);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song1", UriKind.Relative), updatedUris[0]);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Song2", UriKind.Relative), updatedUris[1]);
		}

		[Test]
		public void ChangeDiscUri_UpdatesImagesUriPropertyCorrectly()
		{
			// Arrange

			var image1 = new DiscImage { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Image1", UriKind.Relative) };
			var image2 = new DiscImage { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Image2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Images = { image1, image2 },
			};

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInImageUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), image1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInImageUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), image2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), libraryStructurerStub, Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image1", UriKind.Relative), image1.Uri);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image2", UriKind.Relative), image2.Uri);
		}

		[Test]
		public void ChangeDiscUri_UpdatesImagesInRepositoryCorrectly()
		{
			// Arrange

			var image1 = new DiscImage { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Image1", UriKind.Relative) };
			var image2 = new DiscImage { Uri = new Uri("/SomeDiscStorage/SomeDiscFolder/Image2", UriKind.Relative) };
			var disc = new Disc
			{
				Uri = new Uri("/SomeDiscStorage/SomeDiscFolder", UriKind.Relative),
				Images = { image1, image2 },
			};

			List<Uri> updatedUris = new List<Uri>();
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateDiscImage(Arg.Do<DiscImage>(arg => updatedUris.Add(arg.Uri)));

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.ReplaceDiscPartInImageUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), image1.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image1", UriKind.Relative));
			libraryStructurerStub.ReplaceDiscPartInImageUri(new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative), image2.Uri)
				.Returns(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image2", UriKind.Relative));
			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), libraryStructurerStub, Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.ChangeDiscUri(disc, new Uri("/SomeDiscStorage/SomeNewDiscFolder", UriKind.Relative)).Wait();

			// Assert

			repositoryMock.Received(1).UpdateDiscImage(image1);
			repositoryMock.Received(1).UpdateDiscImage(image2);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image1", UriKind.Relative), updatedUris[0]);
			Assert.AreEqual(new Uri("/SomeDiscStorage/SomeNewDiscFolder/Image2", UriKind.Relative), updatedUris[1]);
		}

		[Test]
		public void DeleteDisc_DeletesDiscSongsFromStorage()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();

			var disc = new Disc
			{
				SongsUnordered = new List<Song> { song1, song2, },
			};

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.DeleteDisc(disc).Wait();

			// Assert

			storageMock.Received(1).DeleteSong(song1);
			storageMock.Received(1).DeleteSong(song2);
		}

		[Test]
		public void DeleteDisc_SavesDeleteTimeInRepositoryForDiscSongs()
		{
			// Arrange

			var song1 = new Song();
			var song2 = new Song();

			var disc = new Disc
			{
				SongsUnordered = new List<Song> { song1, song2 },
			};

			var savedDeleteDateTimes = new List<DateTime?>();
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(arg => savedDeleteDateTimes.Add(arg.DeleteDate)));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			IClock dateTimeStub = Substitute.For<IClock>();
			dateTimeStub.Now.Returns(new DateTime(2017, 10, 20, 12, 07, 08));
			target.DateTimeFacade = dateTimeStub;

			// Act

			target.DeleteDisc(disc).Wait();

			// Assert

			repositoryMock.Received(1).UpdateSong(song1);
			repositoryMock.Received(1).UpdateSong(song2);
			CollectionAssert.AreEqual(Enumerable.Repeat(new DateTime(2017, 10, 20, 12, 07, 08), 2), savedDeleteDateTimes);
		}

		[Test]
		public void DeleteDisc_DeletesDiscImagesFromStorage()
		{
			// Arrange

			var discImage1 = new DiscImage();
			var discImage2 = new DiscImage();

			var disc = new Disc
			{
				Images = { discImage1, discImage2 }
			};

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.DeleteDisc(disc).Wait();

			// Assert

			storageMock.Received(1).DeleteDiscImage(discImage1);
			storageMock.Received(1).DeleteDiscImage(discImage2);
		}

		[Test]
		public void DeleteDisc_DeletesDiscImagesFromRepository()
		{
			// Arrange

			var discImage1 = new DiscImage();
			var discImage2 = new DiscImage();

			var disc = new Disc
			{
				Images = { discImage1, discImage2 }
			};

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.DeleteDisc(disc).Wait();

			// Assert

			repositoryMock.Received(1).DeleteDiscImage(discImage1);
			repositoryMock.Received(1).DeleteDiscImage(discImage2);
		}

		[Test]
		public void SetDiscCoverImage_IfPreviousDiscCoverImageExists_DeletesPreviousImageFromStorage()
		{
			// Arrange

			DiscImage prevImage = new DiscImage { ImageType = DiscImageType.Cover };

			var disc = new Disc
			{
				CoverImage = prevImage
			};

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.SetDiscCoverImage(disc, new ImageInfo()).Wait();

			// Assert

			storageMock.Received(1).DeleteDiscImage(prevImage);
		}

		[Test]
		public void SetDiscCoverImage_IfPreviousDiscCoverImageExists_DeletesPreviousImageFromRepository()
		{
			// Arrange

			DiscImage prevImage = new DiscImage { ImageType = DiscImageType.Cover };

			var disc = new Disc
			{
				CoverImage = prevImage
			};

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.SetDiscCoverImage(disc, new ImageInfo()).Wait();

			// Assert

			repositoryMock.Received(1).DeleteDiscImage(prevImage);
		}

		[Test]
		public void SetDiscCoverImage_FillsNewImageCorrectly()
		{
			// Arrange

			var disc = new Disc
			{
				Uri = new Uri("/SomeDisc", UriKind.Relative)
			};
			var imageInfo = new ImageInfo
			{
				FileName = "ImageFileName.img",
				FileSize = 12345,
			};

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			ILibraryStructurer libraryStructurerStub = Substitute.For<ILibraryStructurer>();
			libraryStructurerStub.GetDiscCoverImageUri(new Uri("/SomeDisc", UriKind.Relative), imageInfo).Returns(new Uri("/SomeDisc/SomeImage", UriKind.Relative));

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("ImageFileName.img").Returns(54321);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), libraryStructurerStub, checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.SetDiscCoverImage(disc, imageInfo).Wait();

			// Assert

			var image = disc.CoverImage;
			Assert.AreEqual(new Uri("/SomeDisc/SomeImage", UriKind.Relative), image.Uri);
			Assert.AreEqual(12345, image.FileSize);
			Assert.AreEqual(54321, image.Checksum);
			Assert.AreSame(disc, image.Disc);
			Assert.AreEqual(DiscImageType.Cover, image.ImageType);
		}

		[Test]
		public void SetDiscCoverImage_StoresNewDiscImageInStorageCorrectly()
		{
			// Arrange

			var disc = new Disc();
			var imageInfo = new ImageInfo
			{
				FileName = "ImageFileName.img"
			};

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.SetDiscCoverImage(disc, imageInfo).Wait();

			// Assert

			storageMock.Received(1).StoreDiscImage("ImageFileName.img", Arg.Any<DiscImage>());
		}

		[Test]
		public void SetDiscCoverImage_AddsNewDiscImageToRepository()
		{
			// Arrange

			var disc = new Disc();
			var imageInfo = new ImageInfo
			{
				FileName = "ImageFileName.img"
			};

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.SetDiscCoverImage(disc, imageInfo).Wait();

			// Assert

			repositoryMock.Received(1).AddDiscImage(Arg.Any<DiscImage>());
		}

		[Test]
		public void AddSongPlayback_AddsNewPlaybackToRepository()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.AddSongPlayback(song, new DateTime(2017, 10, 20, 12, 29, 25)).Wait();

			// Assert

			repositoryMock.Received(1).AddSongPlayback(song, new DateTime(2017, 10, 20, 12, 29, 25));
		}

		[Test]
		public void FixSongTagData_FixesTagDataInSongFileCorrectly()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SongFileName");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.FixSongTagData(song).Wait();

			// Assert

			songTaggerMock.Received(1).FixTagData("SongFileName");
		}

		[Test]
		public void FixSongTagData_UpdatesSongCheckSum()
		{
			// Arrange

			var song = new Song
			{
				Checksum = 54321
			};

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SomeSongFileName");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFileName").Returns(12345);

			int? savedChecksum = null;
			IMusicLibraryRepository repository = Substitute.For<IMusicLibraryRepository>();
			repository.UpdateSong(Arg.Do<Song>(s => savedChecksum = s.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repository, storageStub, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.FixSongTagData(song).Wait();

			// Assert

			Assert.AreEqual(12345, song.Checksum);
			Assert.AreEqual(12345, savedChecksum);
		}

		[Test]
		public void FixSongTagData_CalculatesSongChecksumOnlyAfterSongTagDataIsFixed()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFileForWriting(song).Returns("SongFileName");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			IChecksumCalculator checksumCalculatorMock = Substitute.For<IChecksumCalculator>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), checksumCalculatorMock, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.FixSongTagData(song).Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.FixTagData("SongFileName");
				checksumCalculatorMock.CalculateChecksumForFile("SongFileName");
			});
		}

		[Test]
		public void FixSongTagData_UpdateSongContentInStorage()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryStorage storageMock = Substitute.For<IMusicLibraryStorage>();
			storageMock.GetSongFileForWriting(song).Returns("SongFileName");

			ISongTagger songTaggerMock = Substitute.For<ISongTagger>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock,
				songTaggerMock, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.FixSongTagData(song).Wait();

			// Assert

			Received.InOrder(() =>
			{
				songTaggerMock.Received(1).FixTagData("SongFileName");
				storageMock.Received(1).UpdateSongContent("SongFileName", song);
			});
		}
	}
}
