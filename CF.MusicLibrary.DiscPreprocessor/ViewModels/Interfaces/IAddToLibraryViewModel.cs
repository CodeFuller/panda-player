using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IAddToLibraryViewModel : IPageViewModel
	{
		void SetSongs(IEnumerable<AddedSong> songs);

		void SetDiscsCoverImages(IEnumerable<AddedDiscCoverImage> coverImages);

		Task AddContentToLibrary();
	}
}
