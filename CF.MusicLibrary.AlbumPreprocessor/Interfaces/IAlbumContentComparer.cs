using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor.Interfaces
{
	public interface IAlbumContentComparer
	{
		void SetAlbumsCorrectness(AlbumTreeViewModel ethalonAlbums, AlbumTreeViewModel currentAlbums);
	}
}
