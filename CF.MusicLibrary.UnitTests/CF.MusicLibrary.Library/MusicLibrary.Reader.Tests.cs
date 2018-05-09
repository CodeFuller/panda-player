using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.Library;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.Library
{
	[TestFixture]
	public partial class RepositoryAndStorageMusicLibraryTests
	{
		[Test]
		public void Constructor_IfLibraryRepositoryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(null, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>()));
		}

		[Test]
		public void Constructor_IfLibraryStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), null,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>()));
		}

		[Test]
		public void Constructor_IfSongTaggerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				null, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>()));
		}

		[Test]
		public void Constructor_IfLibraryStructurerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), null, Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>()));
		}

		[Test]
		public void Constructor_IfChecksumCalculatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), null, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>()));
		}

		[Test]
		public void LoadDiscs_ReturnsDiscsFromRepository()
		{
			//	Arrange

			var discs = new[] { new Disc(), new Disc() };

			IMusicLibraryRepository musicLibraryRepositoryStub = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryStub.GetDiscs().Returns(discs.AsEnumerable());

			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryStub, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var returnedDiscs = target.LoadDiscs().Result;

			//	Assert

			Assert.AreEqual(discs, returnedDiscs);
		}

		[Test]
		public void LoadLibrary_ReturnsLibraryWithDiscsFromRepository()
		{
			//	Arrange

			var discs = new[] { new Disc(), new Disc() };

			IMusicLibraryRepository musicLibraryRepositoryStub = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryStub.GetDiscs().Returns(discs.AsEnumerable());

			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryStub, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var library = target.LoadLibrary().Result;

			//	Assert

			CollectionAssert.AreEqual(discs, library.AllDiscs);
		}

		[Test]
		public void GetSongFile_ReturnsSongFileFromStorage()
		{
			//	Arrange

			var song = new Song();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSong.mp3");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var songFileName = target.GetSongFile(song).Result;

			//	Assert

			Assert.AreEqual("SomeSong.mp3", songFileName);
		}

		[Test]
		public void GetSongTagData_ReturnsTagDataFromSongFile()
		{
			//	Arrange

			var song = new Song();
			var tagData = new SongTagData();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSong.mp3");

			ISongTagger songTaggerStub = Substitute.For<ISongTagger>();
			songTaggerStub.GetTagData("SomeSong.mp3").Returns(tagData);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerStub, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var returnedTagData = target.GetSongTagData(song).Result;

			//	Assert

			Assert.AreSame(tagData, returnedTagData);
		}

		[Test]
		public void GetSongTagTypes_ReturnsTagTypesFromSongFile()
		{
			//	Arrange

			var song = new Song();
			var tagTypes = new[]
			{
				SongTagType.Id3V1,
				SongTagType.Id3V2,
			};

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSong.mp3");

			ISongTagger songTaggerStub = Substitute.For<ISongTagger>();
			songTaggerStub.GetTagTypes("SomeSong.mp3").Returns(tagTypes);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerStub, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var returnedTagTypes = target.GetSongTagTypes(song).Result;

			//	Assert

			Assert.AreEqual(tagTypes, returnedTagTypes);
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasCoverImage_ReturnsCoverImageFileFromStorage()
		{
			//	Arrange

			var discImage = new DiscImage { ImageType = DiscImageType.Cover };
			var disc = new Disc { CoverImage = discImage };

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(discImage).Returns("SomeImage.img");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var imageFileName = target.GetDiscCoverImage(disc).Result;

			//	Assert

			Assert.AreEqual("SomeImage.img", imageFileName);
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasNoCoverImage_ReturnsNull()
		{
			//	Arrange

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(Arg.Any<DiscImage>()).Returns("SomeImage.img");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			var imageFileName = target.GetDiscCoverImage(new Disc()).Result;

			//	Assert

			Assert.IsNull(imageFileName);
		}

		[Test]
		public void CheckStorage_ChecksStorageDataConsistencyForAllSongs()
		{
			//	Arrange

			var disc1 = new Disc
			{
				SongsUnordered = new[]
				{
					new Song {Uri = new Uri("/SomeSong11.mp3", UriKind.Relative)},
					new Song {Uri = new Uri("/SomeSong12.mp3", UriKind.Relative)},
				}
			};

			var disc2 = new Disc
			{
				SongsUnordered = new[]
				{
					new Song {Uri = new Uri("/SomeSong21.mp3", UriKind.Relative)},
				}
			};

			var discLibrary = new DiscLibrary(new[] { disc1, disc2 });

			List<Uri> passedUris = null;
			ILibraryStorageInconsistencyRegistrator registrator = Substitute.For<ILibraryStorageInconsistencyRegistrator>();
			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			musicLibraryStorageMock.CheckDataConsistency(Arg.Do<IEnumerable<Uri>>(arg => passedUris = arg.ToList()), registrator, false);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorage(discLibrary, registrator, false).Wait();

			//	Assert

			var expectedUris = new[]
			{
				new Uri("/SomeSong11.mp3", UriKind.Relative),
				new Uri("/SomeSong12.mp3", UriKind.Relative),
				new Uri("/SomeSong21.mp3", UriKind.Relative),
			};

			Assert.AreEqual(expectedUris, passedUris);
		}

		[Test]
		public void CheckStorage_ChecksStorageDataConsistencyForAllDiscImages()
		{
			//	Arrange

			var disc1 = new Disc
			{
				SongsUnordered = new[] { new Song() },
				CoverImage = new DiscImage
				{
					ImageType = DiscImageType.Cover,
					Uri = new Uri("/SomeImage1.img", UriKind.Relative),
				}
			};
			var disc2 = new Disc
			{
				SongsUnordered = new[] { new Song() },
				CoverImage = new DiscImage
				{
					ImageType = DiscImageType.Cover,
					Uri = new Uri("/SomeImage2.img", UriKind.Relative),
				}
			};

			var discLibrary = new DiscLibrary(new[] { disc1, disc2 });

			List<Uri> passedUris = null;
			ILibraryStorageInconsistencyRegistrator registrator = Substitute.For<ILibraryStorageInconsistencyRegistrator>();
			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			musicLibraryStorageMock.CheckDataConsistency(Arg.Do<IEnumerable<Uri>>(arg => passedUris = arg.ToList()), registrator, false);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorage(discLibrary, registrator, false).Wait();

			//	Assert

			CollectionAssert.Contains(passedUris, new Uri("/SomeImage1.img", UriKind.Relative));
			CollectionAssert.Contains(passedUris, new Uri("/SomeImage2.img", UriKind.Relative));
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumDiffersFromStoredInRepository_RegistersStorageInconsistency()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var disc = new Disc { SongsUnordered = new[] { song } };
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(54321);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			//	Act

			target.CheckStorageChecksums(library, registratorMock, false).Wait();

			//	Assert

			registratorMock.Received(1).RegisterInconsistency_ErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumEqualsToStoredInRepository_DoesNotRegisterStorageInconsistency()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var disc = new Disc { SongsUnordered = new[] { song } };
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			//	Act

			target.CheckStorageChecksums(library, registratorMock, false).Wait();

			//	Assert

			registratorMock.DidNotReceive().RegisterInconsistency_ErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumsDifferAndFixFoundIssuesIsTrue_UpdatesSongChecksum()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var disc = new Disc { SongsUnordered = new[] { song } };
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(54321);

			int? savedChecksum = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateSong(Arg.Do<Song>(arg => savedChecksum = arg.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorageChecksums(library, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), true).Wait();

			//	Assert

			Assert.AreEqual(54321, song.Checksum);
			Assert.AreEqual(54321, savedChecksum);
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumsDifferAndFixFoundIssuesIsFalse_DoesNotUpdateSongChecksum()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var disc = new Disc { SongsUnordered = new[] { song } };
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(54321);

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorageChecksums(library, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			//	Assert

			Assert.AreEqual(12345, song.Checksum);
			repositoryMock.DidNotReceive().UpdateSong(Arg.Any<Song>());
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumDiffersFromStoredInRepository_RegistersStorageInconsistency()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { song },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			//	Act

			target.CheckStorageChecksums(library, registratorMock, false).Wait();

			//	Assert

			registratorMock.Received(1).RegisterInconsistency_ErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumEqualsToStoredInRepository_DoesNotRegisterStorageInconsistency()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { song },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(12345);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			//	Act

			target.CheckStorageChecksums(library, registratorMock, false).Wait();

			//	Assert

			registratorMock.DidNotReceive().RegisterInconsistency_ErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumsDifferAndFixFoundIssuesIsTrue_UpdatesSongChecksum()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { song },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			int? savedChecksum = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateDiscImage(Arg.Do<DiscImage>(arg => savedChecksum = arg.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorageChecksums(library, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), true).Wait();

			//	Assert

			Assert.AreEqual(54321, image.Checksum);
			Assert.AreEqual(54321, savedChecksum);
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumsDifferAndFixFoundIssuesIsFalse_DoesNotUpdateSongChecksum()
		{
			//	Arrange

			var song = new Song
			{
				Uri = new Uri("/SomeSong.mp3", UriKind.Relative),
				Checksum = 12345,
			};
			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { song },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSongFile.mp3");
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			//	Act

			target.CheckStorageChecksums(library, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			//	Assert

			Assert.AreEqual(12345, image.Checksum);
			repositoryMock.DidNotReceive().UpdateDiscImage(Arg.Any<DiscImage>());
		}
	}
}
