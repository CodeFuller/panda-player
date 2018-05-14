using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.Tests;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ViewModels
{
	[TestFixture]
	public class AddToLibraryViewModelTests
	{
		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(null, Substitute.For<ISongMediaInfoProvider>(),
				Substitute.For<IWorkshopMusicStorage>(), new DiscPreprocessorSettings().StubOptions()));
		}

		[Test]
		public void Constructor_IfSongMediaInfoProviderArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), null,
				Substitute.For<IWorkshopMusicStorage>(), new DiscPreprocessorSettings().StubOptions()));
		}

		[Test]
		public void Constructor_IfWorkshopMusicStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(),
				Substitute.For<ISongMediaInfoProvider>(), null, new DiscPreprocessorSettings().StubOptions()));
		}

		[Test]
		public void AddContentToLibrary_FillsSongMediaInfoCorrectly()
		{
			// Arrange

			Song addedSong = null;

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();
			musicLibraryMock.AddSong(Arg.Do<Song>(arg => addedSong = arg), Arg.Any<string>());

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(
			new SongMediaInfo
			{
				Size = 12345,
				Bitrate = 256000,
				Duration = TimeSpan.FromSeconds(3600),
			}));

			AddToLibraryViewModel target = new AddToLibraryViewModel(musicLibraryMock, mediaInfoProviderStub,
				Substitute.For<IWorkshopMusicStorage>(), new DiscPreprocessorSettings().StubOptions());
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			// Act

			target.AddContentToLibrary().Wait();

			// Assert

			Assert.AreEqual(12345, addedSong.FileSize);
			Assert.AreEqual(256000, addedSong.Bitrate);
			Assert.AreEqual(TimeSpan.FromSeconds(3600), addedSong.Duration);
		}

		[Test]
		public void AddContentToLibrary_StoresDiscsImagesCorrectly()
		{
			// Arrange

			Disc disc1 = new Disc();
			Disc disc2 = new Disc();
			ImageInfo imageInfo1 = new ImageInfo();
			ImageInfo imageInfo2 = new ImageInfo();
			AddedDiscImage addedImage1 = new AddedDiscImage(disc1, imageInfo1);
			AddedDiscImage addedImage2 = new AddedDiscImage(disc2, imageInfo2);

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			AddToLibraryViewModel target = new AddToLibraryViewModel(musicLibraryMock, mediaInfoProviderStub,
				Substitute.For<IWorkshopMusicStorage>(), new DiscPreprocessorSettings().StubOptions());
			target.SetSongs(Array.Empty<AddedSong>());
			target.SetDiscsImages(new[] { addedImage1, addedImage2 });

			// Act

			target.AddContentToLibrary().Wait();

			// Assert

			musicLibraryMock.Received(1).SetDiscCoverImage(disc1, imageInfo1);
			musicLibraryMock.Received(1).SetDiscCoverImage(disc2, imageInfo2);
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsTrue_DeletesSourceContentCorrectly()
		{
			// Arrange

			ImageInfo imageInfo1 = new ImageInfo { FileName = "DiscCoverImage1.img" };
			ImageInfo imageInfo2 = new ImageInfo { FileName = "DiscCoverImage2.img" };

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			List<string> deletedFiles = null;
			IWorkshopMusicStorage workshopMusicStorageMock = Substitute.For<IWorkshopMusicStorage>();
			workshopMusicStorageMock.DeleteSourceContent(Arg.Do<IEnumerable<string>>(arg => deletedFiles = arg.ToList()));

			var settings = new DiscPreprocessorSettings
			{
				DeleteSourceContentAfterAdding = true
			};

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(),
				mediaInfoProviderStub, workshopMusicStorageMock, settings.StubOptions());
			target.SetSongs(new[]
			{
				new AddedSong(new Song(), @"SomeSongPath\SomeSongFile1.mp3"),
				new AddedSong(new Song(), @"SomeSongPath\SomeSongFile2.mp3"),
			});
			target.SetDiscsImages(new[]
			{
				new AddedDiscImage(new Disc(), imageInfo1),
				new AddedDiscImage(new Disc(), imageInfo2),
			});

			// Act

			target.AddContentToLibrary().Wait();

			// Assert

			Assert.IsNotNull(deletedFiles);
			CollectionAssert.AreEqual(new[]
			{
				@"SomeSongPath\SomeSongFile1.mp3",
				@"SomeSongPath\SomeSongFile2.mp3",
				"DiscCoverImage1.img",
				"DiscCoverImage2.img",
			}, deletedFiles);
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsFalse_DoesNotDeleteSourceContent()
		{
			// Arrange

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			IWorkshopMusicStorage workshopMusicStorageMock = Substitute.For<IWorkshopMusicStorage>();

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(),
				mediaInfoProviderStub, workshopMusicStorageMock, new DiscPreprocessorSettings().StubOptions());
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });
			target.SetDiscsImages(new[] { new AddedDiscImage(new Disc(), new ImageInfo()) });

			// Act

			target.AddContentToLibrary().Wait();

			// Assert

			workshopMusicStorageMock.DidNotReceiveWithAnyArgs().DeleteSourceContent(Arg.Any<IEnumerable<string>>());
		}
	}
}
