using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Facades;
using CF.MusicLibrary.AlbumPreprocessor;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.UnitTests.CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	[TestFixture]
	public class AddToLibraryViewModelTests
	{
		[Test]
		public void AddAlbumsToLibrary_ClearsReadOnlyAttributeBeforeTagging()
		{
			//	Arrange

			AlbumContent albumContent = new AlbumContent(String.Empty, Enumerable.Empty<string>());
			AlbumTreeViewItem albumItem = new AlbumTreeViewItem(albumContent);

			TaggedSongData songData = new TaggedSongData
			{
				SourceFileName = @"SomeSongPath\SomeSongFile.mp3"
			};

			EditAlbumsDetailsViewModel editAlbumsDetailsViewModelStub = Substitute.For<EditAlbumsDetailsViewModel>(
			Substitute.For<IMusicLibrary>(), Substitute.For<IWorkshopMusicStorage>(),
			Substitute.For<IStorageUrlBuilder>(), Substitute.For<IFileSystemFacade>());
			editAlbumsDetailsViewModelStub.Albums.Returns(new ObservableCollection<AddedAlbum>());
			editAlbumsDetailsViewModelStub.Songs.Returns(Enumerable.Repeat(songData, 1));

			EditSongsDetailsViewModel editSongsDetailsViewModelStub = Substitute.For<EditSongsDetailsViewModel>();
			editSongsDetailsViewModelStub.Songs.Returns(new ObservableCollection<SongTagDataViewItem>(Enumerable.Repeat(new SongTagDataViewItem(songData), 1)));

			IWindowService windowServiceStub = Substitute.For<IWindowService>();
			windowServiceStub.ShowEditAlbumsDetailsWindow(editAlbumsDetailsViewModelStub).Returns(true);
			windowServiceStub.ShowEditSongsDetailsWindow(Arg.Any<EditSongsDetailsViewModel>()).Returns(true);
				
			IFileSystemFacade fileSystemMock = Substitute.For<IFileSystemFacade>();

			AddToLibraryViewModel target = new AddToLibraryViewModel(editAlbumsDetailsViewModelStub, editSongsDetailsViewModelStub,
				Substitute.For<ISongTagger>(), windowServiceStub, Substitute.For<IMusicLibrary>(), fileSystemMock);

			//	Act

			target.AddAlbumsToLibrary(Enumerable.Repeat(albumItem, 1)).Wait();

			//	Assert

			fileSystemMock.Received(1).ClearReadOnlyAttribute(@"SomeSongPath\SomeSongFile.mp3");
		}
	}
}
