using System;
using System.Linq;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Common.DiscArt;
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
				Substitute.For<IObjectFactory<IDiscArtImageFile>>(), Substitute.For<IFileSystemFacade>()));
		}

		[Test]
		public void Constructor_IfLibraryStructurerArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(new DiscLibrary(), null,
				Substitute.For<IObjectFactory<IDiscArtImageFile>>(), Substitute.For<IFileSystemFacade>()));
		}

		[Test]
		public void Constructor_IfDiscArtImageFileFactoryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new EditDiscsDetailsViewModel(new DiscLibrary(), Substitute.For<ILibraryStructurer>(),
				null, Substitute.For<IFileSystemFacade>()));
		}

		[Test]
		public void Constructor_IfFileSystemFacadeArgumentIsNull_ThrowsArgumentNullException()
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

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists(@"Workshop\Some Artist\2000 - Some Disc\cover.jpg").Returns(true);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, fileSystemFacadeStub);

			//	Act

			target.SetDiscs(new[] { addedDisc }).Wait();

			//	Assert

			discArtImageFileMock.Received(1).Load(@"Workshop\Some Artist\2000 - Some Disc\cover.jpg", false);
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

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists(Arg.Any<string>()).Returns(false);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, fileSystemFacadeStub);

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

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists(@"Workshop\Some Artist\2000 - Some Disc\cover.jpg").Returns(true);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, fileSystemFacadeStub);
			target.SetDiscs(new[] { addedDisc }).Wait();
			discArtImageFileMock.ClearReceivedCalls();

			//	Act

			target.RefreshContent();

			//	Assert

			discArtImageFileMock.Received(1).Load(@"Workshop\Some Artist\2000 - Some Disc\cover.jpg", false);
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

			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists(Arg.Any<string>()).Returns(false);

			var discLibrary = new DiscLibrary(() => Task.FromResult(Enumerable.Empty<Disc>()));
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, fileSystemFacadeStub);
			target.SetDiscs(new[] { addedDisc }).Wait();
			discArtImageFileMock.ClearReceivedCalls();

			//	Act

			target.RefreshContent();

			//	Assert

			discArtImageFileMock.Received(1).Unload();
		}

		[Test]
		public void DiscCoverImages_ReturnsCoverImagesForAllDiscs()
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
			var target = new EditDiscsDetailsViewModel(discLibrary, Substitute.For<ILibraryStructurer>(), discArtImageFileFactoryStub, Substitute.For<IFileSystemFacade>());
			target.SetDiscs(new[] { addedDisc1, addedDisc2 }).Wait();

			//	Act

			var addedDiscCoverImages = target.DiscCoverImages.ToList();

			//	Assert

			Assert.AreEqual(2, addedDiscCoverImages.Count);
			Assert.AreEqual("DiscCover1.jpg", addedDiscCoverImages[0].CoverImageFileName);
			Assert.AreEqual("DiscCover2.jpg", addedDiscCoverImages[1].CoverImageFileName);
		}
	}
}
