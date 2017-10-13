using System;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	[TestFixture]
	public class EditDiscsDetailsViewModelTests
	{
		[Test]
		public void Constructor_IfDiscLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(null, Substitute.For<ILibraryStructurer>(),
				Substitute.For<IObjectFactory<IDiscArtImageFile>>(), Substitute.For<IDiscArtFileStorage>()));
		}

		[Test]
		public void Constructor_IfLibraryStructurerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(new DiscLibrary(), null,
				Substitute.For<IObjectFactory<IDiscArtImageFile>>(), Substitute.For<IDiscArtFileStorage>()));
		}

		[Test]
		public void Constructor_IfDiscArtImageFileFactoryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(),
				null, Substitute.For<IDiscArtFileStorage>()));
		}

		[Test]
		public void Constructor_IfDiscArtFileStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(),
				Substitute.For<IObjectFactory<IDiscArtImageFile>>(), null));
		}

		[Test]
		public void SetDiscs_IfDiscCoverImageFileExists_SetsDiscCoverImageToThisFile()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileMock);

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"Workshop\Some Artist\2000 - Some Disc").Returns(@"Workshop\Some Artist\2000 - Some Disc\SomeCover.img");

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, discArtFileStorageStub);

			//	Act

			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load(@"Workshop\Some Artist\2000 - Some Disc\SomeCover.img", false);
		}

		[Test]
		public void SetDiscs_IfDiscCoverImageFileDoesNotExist_DoesNotLoadDiscCoverImage()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileMock);

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"Workshop\Some Artist\2000 - Some Disc").Returns((string)null);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, discArtFileStorageStub);

			//	Act

			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Assert

			discArtImageFileMock.DidNotReceive().Load(Arg.Any<string>(), Arg.Any<bool>());
		}

		[Test]
		public void RefreshContent_IfDiscCoverImageFileExists_SetsDiscCoverImageToThisFile()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileMock);

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"Workshop\Some Artist\2000 - Some Disc").Returns(@"Workshop\Some Artist\2000 - Some Disc\SomeCover.img");

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, discArtFileStorageStub);

			//	Act

			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load(@"Workshop\Some Artist\2000 - Some Disc\SomeCover.img", false);
		}

		[Test]
		public void RefreshContent_IfDiscCoverImageFileDoesNotExist_UnsetsDiscCoverImage()
		{
			//	Arrange

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileMock = Substitute.For<IDiscArtImageFile>();

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileMock);

			IDiscArtFileStorage discArtFileStorageStub = Substitute.For<IDiscArtFileStorage>();
			discArtFileStorageStub.GetDiscCoverImageFileName(@"Workshop\Some Artist\2000 - Some Disc").Returns((string)null);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, discArtFileStorageStub);

			//	Act

			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Unload();
		}

		[Test]
		public void DiscCoverImages_ReturnsFilledCoverImages()
		{
			//	Arrange

			var addedDisc1 = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			var addedDisc2 = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageFileName.Returns("DiscCover1.jpg", "DiscCover2.jpg");

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileStub);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, Substitute.For<IDiscArtFileStorage>());
			target.SetDiscs(new[] { addedDisc1, addedDisc2 }).Wait();

			//	Act

			var addedDiscCoverImages = target.DiscCoverImages.ToList();

			//	Assert

			Assert.AreEqual(2, addedDiscCoverImages.Count);
			Assert.AreEqual("DiscCover1.jpg", addedDiscCoverImages[0].CoverImageFileName);
			Assert.AreEqual("DiscCover2.jpg", addedDiscCoverImages[1].CoverImageFileName);
		}

		[Test]
		public void DiscCoverImages_DoesNotReturnDiscCoverImagesForExistingDiscs()
		{
			//	Arrange

			var existingDisc = new Disc
			{
				Uri = new Uri("/Some Artist/2000 - Some Disc", UriKind.Relative),
				SongsUnordered = new[] { new Song() },
			};

			var addedDisc = new AddedDiscInfo(new AddedSongInfo[] { })
			{
				Title = "Some Disc",
				UriWithinStorage = new Uri("/Some Artist/2000 - Some Disc", UriKind.Relative),
				SourcePath = @"Workshop\Some Artist\2000 - Some Disc",
				DiscType = DsicType.ArtistDisc,
				Artist = "Some Artist",
			};

			IDiscArtImageFile discArtImageFileStub = Substitute.For<IDiscArtImageFile>();
			discArtImageFileStub.ImageIsValid.Returns(true);
			discArtImageFileStub.ImageFileName.Returns("DiscCover.jpg");

			IObjectFactory<IDiscArtImageFile> discArtImageFileFactoryStub = Substitute.For<IObjectFactory<IDiscArtImageFile>>();
			discArtImageFileFactoryStub.CreateInstance().Returns(discArtImageFileStub);

			var discLibrary = new DiscLibrary(() => Task.FromResult(new [] { existingDisc }.AsEnumerable()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, Substitute.For<IDiscArtFileStorage>());
			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Act

			var addedDiscCoverImages = target.DiscCoverImages.ToList();

			//	Assert

			Assert.IsEmpty(addedDiscCoverImages);
		}
	}
}
