using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IAddToLibraryViewModel : IPageViewModel
	{
		void SetSongs(IEnumerable<AddedSong> songs);

		void SetDiscsImages(IEnumerable<AddedDiscImage> images);

		Task AddContentToLibrary();
	}
}
