using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IAddToLibraryViewModel : IPageViewModel
	{
		void SetSongsTagData(IEnumerable<TaggedSongData> tagData);

		void SetAlbumCoverImages(IEnumerable<AddedAlbumCoverImage> coverImages);

		Task AddContentToLibrary();
	}
}
