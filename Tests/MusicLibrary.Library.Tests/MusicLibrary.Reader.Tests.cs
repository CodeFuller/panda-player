using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Media;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.Library.Tests
{
	[TestFixture]
	public partial class RepositoryAndStorageMusicLibraryTests
	{
		[Test]
		public void LoadDiscs_ReturnsDiscsFromRepository()
		{
			// Arrange

			var discs = new[] { new Disc(), new Disc() };

			IMusicLibraryRepository musicLibraryRepositoryStub = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryStub.GetDiscs().Returns(discs.AsEnumerable());

			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryStub, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var returnedDiscs = target.LoadDiscs().Result;

			// Assert

			Assert.AreEqual(discs, returnedDiscs);
		}

		[Test]
		public void LoadLibrary_ReturnsLibraryWithDiscsFromRepository()
		{
			// Arrange

			var discs = new[] { new Disc(), new Disc() };

			IMusicLibraryRepository musicLibraryRepositoryStub = Substitute.For<IMusicLibraryRepository>();
			musicLibraryRepositoryStub.GetDiscs().Returns(discs.AsEnumerable());

			var target = new RepositoryAndStorageMusicLibrary(musicLibraryRepositoryStub, Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var library = target.LoadLibrary().Result;

			// Assert

			CollectionAssert.AreEqual(discs, library.AllDiscs);
		}

		[Test]
		public void GetSongFile_ReturnsSongFileFromStorage()
		{
			// Arrange

			var song = new Song();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSong.mp3");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var songFileName = target.GetSongFile(song).Result;

			// Assert

			Assert.AreEqual("SomeSong.mp3", songFileName);
		}

		[Test]
		public void GetSongTagData_ReturnsTagDataFromSongFile()
		{
			// Arrange

			var song = new Song();
			var tagData = new SongTagData();

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetSongFile(song).Returns("SomeSong.mp3");

			ISongTagger songTaggerStub = Substitute.For<ISongTagger>();
			songTaggerStub.GetTagData("SomeSong.mp3").Returns(tagData);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				songTaggerStub, Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var returnedTagData = target.GetSongTagData(song).Result;

			// Assert

			Assert.AreSame(tagData, returnedTagData);
		}

		[Test]
		public void GetSongTagTypes_ReturnsTagTypesFromSongFile()
		{
			// Arrange

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

			// Act

			var returnedTagTypes = target.GetSongTagTypes(song).Result;

			// Assert

			Assert.AreEqual(tagTypes, returnedTagTypes);
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasCoverImage_ReturnsCoverImageFileFromStorage()
		{
			// Arrange

			var discImage = new DiscImage { ImageType = DiscImageType.Cover };
			var disc = new Disc { CoverImage = discImage };

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(discImage).Returns("SomeImage.img");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var imageFileName = target.GetDiscCoverImage(disc).Result;

			// Assert

			Assert.AreEqual("SomeImage.img", imageFileName);
		}

		[Test]
		public void GetDiscCoverImage_IfDiscHasNoCoverImage_ReturnsNull()
		{
			// Arrange

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(Arg.Any<DiscImage>()).Returns("SomeImage.img");

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			var imageFileName = target.GetDiscCoverImage(new Disc()).Result;

			// Assert

			Assert.IsNull(imageFileName);
		}

		[Test]
		public void CheckStorage_ChecksStorageDataConsistencyForAllSongsWithinCheckScope()
		{
			// Arrange

			var disc1 = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { Uri = new Uri("/SomeSong11.mp3", UriKind.Relative) },
					new Song { Uri = new Uri("/SomeSong12.mp3", UriKind.Relative) },
				},
			};

			var disc2 = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { Uri = new Uri("/SomeSong21.mp3", UriKind.Relative) },
				},
			};

			var disc3 = new Disc
			{
				SongsUnordered = new[]
				{
					new Song { Uri = new Uri("/SomeSong31.mp3", UriKind.Relative) },
				},
			};

			var discLibrary = new DiscLibrary(new[] { disc1, disc2, disc3 });

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(disc1).Returns(true);
			checkScopeStub.Contains(disc2).Returns(true);
			checkScopeStub.Contains(disc3).Returns(false);

			List<Uri> passedUris = null;
			ILibraryStorageInconsistencyRegistrator registrator = Substitute.For<ILibraryStorageInconsistencyRegistrator>();
			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			musicLibraryStorageMock.CheckDataConsistency(Arg.Do<IEnumerable<Uri>>(arg => passedUris = arg.ToList()), Arg.Any<IUriCheckScope>(), registrator, false);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorage(discLibrary, checkScopeStub, registrator, false).Wait();

			// Assert

			var expectedUris = new[]
			{
				new Uri("/SomeSong11.mp3", UriKind.Relative),
				new Uri("/SomeSong12.mp3", UriKind.Relative),
				new Uri("/SomeSong21.mp3", UriKind.Relative),
			};

			Assert.AreEqual(expectedUris, passedUris);
		}

		[Test]
		public void CheckStorage_ChecksStorageDataConsistencyForAllDiscImagesWithinCheckScope()
		{
			// Arrange

			var disc1 = new Disc
			{
				SongsUnordered = new[] { new Song() },
				CoverImage = new DiscImage
				{
					ImageType = DiscImageType.Cover,
					Uri = new Uri("/SomeImage1.img", UriKind.Relative),
				},
			};
			var disc2 = new Disc
			{
				SongsUnordered = new[] { new Song() },
				CoverImage = new DiscImage
				{
					ImageType = DiscImageType.Cover,
					Uri = new Uri("/SomeImage2.img", UriKind.Relative),
				},
			};
			var disc3 = new Disc
			{
				SongsUnordered = new[] { new Song() },
				CoverImage = new DiscImage
				{
					ImageType = DiscImageType.Cover,
					Uri = new Uri("/SomeImage3.img", UriKind.Relative),
				},
			};

			var discLibrary = new DiscLibrary(new[] { disc1, disc2, disc3 });

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(disc1).Returns(true);
			checkScopeStub.Contains(disc2).Returns(true);
			checkScopeStub.Contains(disc3).Returns(false);

			List<Uri> passedUris = null;
			ILibraryStorageInconsistencyRegistrator registrator = Substitute.For<ILibraryStorageInconsistencyRegistrator>();
			IMusicLibraryStorage musicLibraryStorageMock = Substitute.For<IMusicLibraryStorage>();
			musicLibraryStorageMock.CheckDataConsistency(Arg.Do<IEnumerable<Uri>>(arg => passedUris = arg.ToList()), Arg.Any<IUriCheckScope>(), registrator, false);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), musicLibraryStorageMock,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorage(discLibrary, StubFullScope(), registrator, false).Wait();

			// Assert

			CollectionAssert.Contains(passedUris, new Uri("/SomeImage1.img", UriKind.Relative));
			CollectionAssert.Contains(passedUris, new Uri("/SomeImage2.img", UriKind.Relative));
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumDiffersFromStoredInRepository_RegistersStorageInconsistency()
		{
			// Arrange

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

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), registratorMock, false).Wait();

			// Assert

			registratorMock.Received(1).RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumEqualsToStoredInRepository_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

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

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), registratorMock, false).Wait();

			// Assert

			registratorMock.DidNotReceive().RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumsDifferAndFixFoundIssuesIsTrue_UpdatesSongChecksum()
		{
			// Arrange

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

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), Substitute.For<ILibraryStorageInconsistencyRegistrator>(), true).Wait();

			// Assert

			Assert.AreEqual(54321, song.Checksum);
			Assert.AreEqual(54321, savedChecksum);
		}

		[Test]
		public void CheckStorageChecksums_IfSongChecksumsDifferAndFixFoundIssuesIsFalse_DoesNotUpdateSongChecksum()
		{
			// Arrange

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

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			// Assert

			Assert.AreEqual(12345, song.Checksum);
			repositoryMock.DidNotReceive().UpdateSong(Arg.Any<Song>());
		}

		[Test]
		public void CheckStorageChecksums_SkipsSongsOutsideCheckScope()
		{
			// Arrange

			var song11 = new Song();
			var song12 = new Song();
			var disc1 = new Disc { SongsUnordered = new[] { song11, song12 } };

			var song21 = new Song();
			var disc2 = new Disc { SongsUnordered = new[] { song21 } };

			var library = new DiscLibrary(new[] { disc1, disc2 });

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(disc1).Returns(true);
			checkScopeStub.Contains(song11).Returns(true);
			checkScopeStub.Contains(song12).Returns(false);
			checkScopeStub.Contains(disc2).Returns(false);
			checkScopeStub.Contains(song21).Returns(true);

			var storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorageChecksums(library, checkScopeStub, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			// Assert

			storageMock.Received(1).GetSongFile(song11);
			storageMock.DidNotReceive().GetSongFile(song12);
			storageMock.DidNotReceive().GetSongFile(song21);
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumDiffersFromStoredInRepository_RegistersStorageInconsistency()
		{
			// Arrange

			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { new Song { Checksum = 0 } },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), registratorMock, false).Wait();

			// Assert

			registratorMock.Received(1).RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumEqualsToStoredInRepository_DoesNotRegisterStorageInconsistency()
		{
			// Arrange

			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { new Song { Checksum = 0 } },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(12345);

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			ILibraryStorageInconsistencyRegistrator registratorMock = Substitute.For<ILibraryStorageInconsistencyRegistrator>();

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), registratorMock, false).Wait();

			// Assert

			registratorMock.DidNotReceive().RegisterErrorInStorageData(Arg.Any<string>());
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumsDifferAndFixFoundIssuesIsTrue_UpdatesImageChecksum()
		{
			// Arrange

			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { new Song() },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			int? savedChecksum = null;
			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();
			repositoryMock.UpdateDiscImage(Arg.Do<DiscImage>(arg => savedChecksum = arg.Checksum));

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), Substitute.For<ILibraryStorageInconsistencyRegistrator>(), true).Wait();

			// Assert

			Assert.AreEqual(54321, image.Checksum);
			Assert.AreEqual(54321, savedChecksum);
		}

		[Test]
		public void CheckStorageChecksums_IfDiscImageChecksumsDifferAndFixFoundIssuesIsFalse_DoesNotUpdateImageChecksum()
		{
			// Arrange

			var image = new DiscImage
			{
				ImageType = DiscImageType.Cover,
				Checksum = 12345,
			};
			var disc = new Disc
			{
				CoverImage = image,
				SongsUnordered = new[] { new Song() },
			};
			var library = new DiscLibrary(new[] { disc });

			IMusicLibraryStorage storageStub = Substitute.For<IMusicLibraryStorage>();
			storageStub.GetDiscImageFile(image).Returns("SomeImage.img");

			IChecksumCalculator checksumCalculatorStub = Substitute.For<IChecksumCalculator>();
			checksumCalculatorStub.CalculateChecksumForFile("SomeSongFile.mp3").Returns(12345);
			checksumCalculatorStub.CalculateChecksumForFile("SomeImage.img").Returns(54321);

			IMusicLibraryRepository repositoryMock = Substitute.For<IMusicLibraryRepository>();

			var target = new RepositoryAndStorageMusicLibrary(repositoryMock, storageStub,
				Substitute.For<ISongTagger>(), Substitute.For<ILibraryStructurer>(), checksumCalculatorStub, Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorageChecksums(library, StubFullScope(), Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			// Assert

			Assert.AreEqual(12345, image.Checksum);
			repositoryMock.DidNotReceive().UpdateDiscImage(Arg.Any<DiscImage>());
		}

		private IUriCheckScope StubFullScope()
		{
			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(Arg.Any<Disc>()).Returns(true);
			checkScopeStub.Contains(Arg.Any<Song>()).Returns(true);
			return checkScopeStub;
		}

		[Test]
		public void CheckStorageChecksums_SkipsImagesOutsideCheckScope()
		{
			// Arrange

			var image1 = new DiscImage { ImageType = DiscImageType.Cover };
			var disc1 = new Disc
			{
				CoverImage = image1,
				SongsUnordered = new[] { new Song() },
			};

			var image2 = new DiscImage { ImageType = DiscImageType.Cover };
			var disc2 = new Disc
			{
				CoverImage = image2,
				SongsUnordered = new[] { new Song() },
			};

			var library = new DiscLibrary(new[] { disc1, disc2 });

			var checkScopeStub = Substitute.For<IUriCheckScope>();
			checkScopeStub.Contains(disc1).Returns(true);
			checkScopeStub.Contains(disc2).Returns(false);

			var storageMock = Substitute.For<IMusicLibraryStorage>();

			var target = new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), storageMock, Substitute.For<ISongTagger>(),
				Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>(), Substitute.For<ILogger<RepositoryAndStorageMusicLibrary>>());

			// Act

			target.CheckStorageChecksums(library, checkScopeStub, Substitute.For<ILibraryStorageInconsistencyRegistrator>(), false).Wait();

			// Assert

			storageMock.Received(1).GetDiscImageFile(image1);
			storageMock.DidNotReceive().GetDiscImageFile(image2);
		}
	}
}
