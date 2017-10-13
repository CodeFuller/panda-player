using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	[TestFixture]
	public class AddToLibraryViewModelTests
	{
		[Test]
		public void Constructor_IfMusicLibraryArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(null, Substitute.For<ISongMediaInfoProvider>(), Substitute.For<IWorkshopMusicStorage>(), false));
		}

		[Test]
		public void Constructor_IfSongMediaInfoProviderArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), null, Substitute.For<IWorkshopMusicStorage>(), false));
		}

		[Test]
		public void Constructor_IfWorkshopMusicStorageArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), Substitute.For<ISongMediaInfoProvider>(), null, false));
		}

		[Test]
		public void AddContentToLibrary_FillsSongMediaInfoCorrectly()
		{
			//	Arrange

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

			AddToLibraryViewModel target = new AddToLibraryViewModel(musicLibraryMock, mediaInfoProviderStub, Substitute.For<IWorkshopMusicStorage>(), false);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			Assert.AreEqual(12345, addedSong.FileSize);
			Assert.AreEqual(256000, addedSong.Bitrate);
			Assert.AreEqual(TimeSpan.FromSeconds(3600), addedSong.Duration);
		}

		[Test]
		public void AddContentToLibrary_StoresDiscsCoverImagesCorrectly()
		{
			//	Arrange

			Disc disc1 = new Disc();
			Disc disc2 = new Disc();
			AddedDiscCoverImage addedCover1 = new AddedDiscCoverImage(disc1, "DiscCoverImage1.jpg");
			AddedDiscCoverImage addedCover2 = new AddedDiscCoverImage(disc2, "DiscCoverImage2.jpg");

			IMusicLibrary musicLibraryMock = Substitute.For<IMusicLibrary>();

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			AddToLibraryViewModel target = new AddToLibraryViewModel(musicLibraryMock, mediaInfoProviderStub, Substitute.For<IWorkshopMusicStorage>(), false);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });
			target.SetDiscsCoverImages(new[] { addedCover1, addedCover2 });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			musicLibraryMock.Received(1).SetDiscCoverImage(disc1, "DiscCoverImage1.jpg");
			musicLibraryMock.Received(1).SetDiscCoverImage(disc2, "DiscCoverImage2.jpg");
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsTrue_DeletesSourceContentCorrectly()
		{
			//	Arrange

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			List<string> deletedFiles = null;
			IWorkshopMusicStorage workshopMusicStorageMock = Substitute.For<IWorkshopMusicStorage>();
			workshopMusicStorageMock.DeleteSourceContent(Arg.Do<IEnumerable<string>>(arg => deletedFiles = arg.ToList()));

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), mediaInfoProviderStub, workshopMusicStorageMock, true);
			target.SetSongs(new[]
			{
				new AddedSong(new Song(), @"SomeSongPath\SomeSongFile1.mp3"),
				new AddedSong(new Song(), @"SomeSongPath\SomeSongFile2.mp3"),
			});
			target.SetDiscsCoverImages(new[]
			{
				new AddedDiscCoverImage(new Disc(), @"DiscCoverImage1.jpg"),
				new AddedDiscCoverImage(new Disc(), @"DiscCoverImage2.jpg"),
			});

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			Assert.IsNotNull(deletedFiles);
			CollectionAssert.AreEqual(new[]
			{
				@"SomeSongPath\SomeSongFile1.mp3",
				@"SomeSongPath\SomeSongFile2.mp3",
				@"DiscCoverImage1.jpg",
				@"DiscCoverImage2.jpg",
			}, deletedFiles);
		}

		[Test]
		public void AddContentToLibrary_IfDeleteSourceContentIsFalse_DoesNotDeleteSourceContent()
		{
			//	Arrange

			ISongMediaInfoProvider mediaInfoProviderStub = Substitute.For<ISongMediaInfoProvider>();
			mediaInfoProviderStub.GetSongMediaInfo(Arg.Any<string>()).Returns(Task.FromResult(new SongMediaInfo()));

			IWorkshopMusicStorage workshopMusicStorageMock = Substitute.For<IWorkshopMusicStorage>();

			AddToLibraryViewModel target = new AddToLibraryViewModel(Substitute.For<IMusicLibrary>(), mediaInfoProviderStub, workshopMusicStorageMock, false);
			target.SetSongs(new[] { new AddedSong(new Song(), @"SomeSongPath\SomeSongFile.mp3") });

			//	Act

			target.AddContentToLibrary().Wait();

			//	Assert

			workshopMusicStorageMock.DidNotReceiveWithAnyArgs().DeleteSourceContent(Arg.Any<IEnumerable<string>>());
		}
	}
}
