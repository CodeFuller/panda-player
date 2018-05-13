using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.ViewModels
{
	[TestFixture]
	public class ApplicationViewModelTests
	{
		[Test]
		public void Constructor_IfEditSourceContentViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(null, Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSourceDiscImagesViewModel>(), Substitute.For<IEditSongsDetailsViewModel>(), Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfEditDiscsDetailsViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), null,
				Substitute.For<IEditSourceDiscImagesViewModel>(), Substitute.For<IEditSongsDetailsViewModel>(), Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfEditSourceDiscImagesViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				null, Substitute.For<IEditSongsDetailsViewModel>(), Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfEditSongsDetailsViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSourceDiscImagesViewModel>(), null, Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfAddToLibraryViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSourceDiscImagesViewModel>(), Substitute.For<IEditSongsDetailsViewModel>(), null));
		}

		[Test]
		public void SwitchToNextPage_WhenSwitchesToAddToLibraryViewModel_SetSongsFromEditSongsDetailsViewModel()
		{
			// Arrange

			var addedSong1 = new AddedSong(new Song(), "SomeSong1.mp3");
			var addedSong2 = new AddedSong(new Song(), "SomeSong2.mp3");

			SongViewItem[] songItems =
			{
				new SongViewItem(addedSong1),
				new SongViewItem(addedSong2),
			};

			IEditSongsDetailsViewModel editSongsDetailsViewModelStub = Substitute.For<IEditSongsDetailsViewModel>();
			editSongsDetailsViewModelStub.Songs.Returns(new ObservableCollection<SongViewItem>(songItems));

			List<AddedSong> setSongs = null;
			IAddToLibraryViewModel addToLibraryViewModelMock = Substitute.For<IAddToLibraryViewModel>();
			addToLibraryViewModelMock.SetSongs(Arg.Do<IEnumerable<AddedSong>>(arg => setSongs = arg.ToList()));

			var target = new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSourceDiscImagesViewModel>(), editSongsDetailsViewModelStub, addToLibraryViewModelMock);

			// Switch to IEditDiscsDetailsViewModel
			target.SwitchToNextPage().Wait();

			// Switch to IEditSourceDiscImagesViewModel
			target.SwitchToNextPage().Wait();

			// Switch to IEditSongsDetailsViewModel
			target.SwitchToNextPage().Wait();

			// Act

			target.SwitchToNextPage().Wait();

			// Assert

			Assert.IsNotNull(setSongs);
			CollectionAssert.AreEqual(new[] { addedSong1, addedSong2 }, setSongs);
		}

		[Test]
		public void SwitchToNextPage_WhenSwitchesToAddToLibraryViewModel_SetDiscsImagesFromEditSourceDiscsImagesViewModel()
		{
			// Arrange

			AddedDiscImage discImage1 = new AddedDiscImage(new Disc(), new ImageInfo());
			AddedDiscImage discImage2 = new AddedDiscImage(new Disc(), new ImageInfo());

			IEditSourceDiscImagesViewModel editSourceDiscsImagesViewModelStub = Substitute.For<IEditSourceDiscImagesViewModel>();
			editSourceDiscsImagesViewModelStub.AddedImages.Returns(new[] { discImage1, discImage2 });

			IEditSongsDetailsViewModel editSongsDetailsViewModelStub = Substitute.For<IEditSongsDetailsViewModel>();
			editSongsDetailsViewModelStub.Songs.Returns(new ObservableCollection<SongViewItem>());

			List<AddedDiscImage> setImages = null;
			IAddToLibraryViewModel addToLibraryViewModelMock = Substitute.For<IAddToLibraryViewModel>();
			addToLibraryViewModelMock.SetDiscsImages(Arg.Do<IEnumerable<AddedDiscImage>>(arg => setImages = arg.ToList()));

			var target = new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				editSourceDiscsImagesViewModelStub, editSongsDetailsViewModelStub, addToLibraryViewModelMock);

			// Switch to IEditDiscsDetailsViewModel
			target.SwitchToNextPage().Wait();

			// Switch to IEditSourceDiscImagesViewModel
			target.SwitchToNextPage().Wait();

			// Switch to IEditSongsDetailsViewModel
			target.SwitchToNextPage().Wait();

			// Act

			target.SwitchToNextPage().Wait();

			// Assert

			Assert.IsNotNull(setImages);
			CollectionAssert.AreEqual(new[] { discImage1, discImage2 }, setImages);
		}
	}
}
