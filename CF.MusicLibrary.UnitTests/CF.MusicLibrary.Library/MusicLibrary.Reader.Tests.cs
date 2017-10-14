using System;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Library;
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
				Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>()));
		}

		[Test]
		public void Constructor_IfLibraryStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), null,
				Substitute.For<ILibraryStructurer>(), Substitute.For<IChecksumCalculator>()));
		}

		[Test]
		public void Constructor_IfLibraryStructurerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				null, Substitute.For<IChecksumCalculator>()));
		}

		[Test]
		public void Constructor_IfChecksumCalculatorArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new RepositoryAndStorageMusicLibrary(Substitute.For<IMusicLibraryRepository>(), Substitute.For<IMusicLibraryStorage>(),
				Substitute.For<ILibraryStructurer>(), null));
		}
	}
}
