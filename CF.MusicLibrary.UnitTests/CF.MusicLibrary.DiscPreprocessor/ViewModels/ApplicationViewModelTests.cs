using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	[TestFixture]
	public class ApplicationViewModelTests
	{
		[Test]
		public void Constructor_IfEditSourceContentViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(null, Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSongsDetailsViewModel>(), Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfEditDiscsesDetailsViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), null,
				Substitute.For<IEditSongsDetailsViewModel>(), Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfEditSongsDetailsViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				null, Substitute.For<IAddToLibraryViewModel>()));
		}

		[Test]
		public void Constructor_IfAddToLibraryViewModelArgumentIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), Substitute.For<IEditDiscsDetailsViewModel>(),
				Substitute.For<IEditSongsDetailsViewModel>(), null));
		}

		[Test]
		public void SwitchToNextPage_WhenSwitchesToAddToLibraryViewModel_SetSongsFromEditSongsDetailsViewModel()
		{
			//	Arrange

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
				editSongsDetailsViewModelStub, addToLibraryViewModelMock);

			//	Switch to IEditDiscsDetailsViewModel
			target.SwitchToNextPage().Wait();
			//	Switch to IEditSongsDetailsViewModel
			target.SwitchToNextPage().Wait();

			//	Act

			target.SwitchToNextPage().Wait();

			//	Assert

			Assert.IsNotNull(setSongs);
			CollectionAssert.AreEqual(new[] { addedSong1, addedSong2}, setSongs);
		}

		[Test]
		public void SwitchToNextPage_WhenSwitchesToAddToLibraryViewModel_SetDiscsCoverImagesFromEditDiscsesDetailsViewModel()
		{
			//	Arrange

			AddedDiscCoverImage discCoverImage1 = new AddedDiscCoverImage(new Disc(), "SomeCover1.jpg");
			AddedDiscCoverImage discCoverImage2 = new AddedDiscCoverImage(new Disc(), "SomeCover2.jpg");

			IEditDiscsDetailsViewModel editDiscsDetailsViewModelStub = Substitute.For<IEditDiscsDetailsViewModel>();
			editDiscsDetailsViewModelStub.DiscCoverImages.Returns(new[] { discCoverImage1, discCoverImage2 });

			IEditSongsDetailsViewModel editSongsDetailsViewModelStub = Substitute.For<IEditSongsDetailsViewModel>();
			editSongsDetailsViewModelStub.Songs.Returns(new ObservableCollection<SongViewItem>());

			List<AddedDiscCoverImage> setCoverImages = null;
			IAddToLibraryViewModel addToLibraryViewModelMock = Substitute.For<IAddToLibraryViewModel>();
			addToLibraryViewModelMock.SetDiscsCoverImages(Arg.Do<IEnumerable<AddedDiscCoverImage>>(arg => setCoverImages = arg.ToList()));

			var target = new ApplicationViewModel(Substitute.For<IEditSourceContentViewModel>(), editDiscsDetailsViewModelStub,
				editSongsDetailsViewModelStub, addToLibraryViewModelMock);

			//	Switch to IEditDiscsDetailsViewModel
			target.SwitchToNextPage().Wait();
			//	Switch to IEditSongsDetailsViewModel
			target.SwitchToNextPage().Wait();

			//	Act

			target.SwitchToNextPage().Wait();

			//	Assert

			Assert.IsNotNull(setCoverImages);
			CollectionAssert.AreEqual(new[] { discCoverImage1, discCoverImage2 }, setCoverImages);
		}
	}
}
