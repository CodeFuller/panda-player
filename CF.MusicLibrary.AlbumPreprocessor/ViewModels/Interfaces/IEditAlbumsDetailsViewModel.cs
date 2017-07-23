using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.SourceContent;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IEditAlbumsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<AddedAlbum> Albums { get; }

		IEnumerable<TaggedSongData> Songs { get; }

		IEnumerable<AddedAlbumCoverImage> AlbumCoverImages { get; }

		Task SetAlbums(IEnumerable<AlbumTreeViewItem> albums);
	}
}
