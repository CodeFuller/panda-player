using CF.MusicLibrary.AlbumPreprocessor.ViewModels.SourceContent;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IEditSourceContentViewModel : IPageViewModel
	{
		AlbumTreeViewModel CurrentAlbums { get; }

		void LoadDefaultContent();
	}
}
