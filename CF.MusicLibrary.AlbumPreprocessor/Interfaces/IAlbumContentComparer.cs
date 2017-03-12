using System.Collections.ObjectModel;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor.Interfaces
{
	public interface IAlbumContentComparer
	{
		void SetAlbumsCorrectness(Collection<AlbumContent> ethalonAlbums, Collection<AlbumTreeViewItem> currentAlbums);
	}
}
