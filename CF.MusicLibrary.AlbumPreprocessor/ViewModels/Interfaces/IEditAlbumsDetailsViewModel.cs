using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IEditAlbumsDetailsViewModel
	{
		ObservableCollection<AddedAlbum> Albums { get; }

		IEnumerable<TaggedSongData> Songs { get; }

		IEnumerable<AddedAlbumCoverImage> AlbumCoverImages { get; }

		bool RequiredDataIsFilled { get; }

		Task SetAlbums(IEnumerable<AlbumTreeViewItem> albums);
	}
}
