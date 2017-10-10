using System;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.DiscPreprocessor;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor
{
	[TestFixture]
	public class SongFileFilterTests
	{
		[Test]
		public void Constructor_IfDiscArtFileStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SongFileFilter(null));
		}

		[Test]
		public void IsSongFile_IfFileIsCoverImage_ReturnsFalse()
		{
			//	Arrange

			IDiscArtFileStorage discArtFileStorage = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorage.IsCoverImageFile("SomeFilePath").Returns(true);

			var target = new SongFileFilter(discArtFileStorage);

			//	Act

			bool isSongFile = target.IsSongFile("SomeFilePath");

			//	Assert

			Assert.IsFalse(isSongFile);
		}

		[Test]
		public void IsSongFile_IfFileIsNotCoverImage_ReturnsTrue()
		{
			//	Arrange

			IDiscArtFileStorage discArtFileStorage = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorage.IsCoverImageFile("SomeFilePath").Returns(false);

			var target = new SongFileFilter(discArtFileStorage);

			//	Act

			bool isSongFile = target.IsSongFile("SomeFilePath");

			//	Assert

			Assert.IsTrue(isSongFile);
		}
	}
}
