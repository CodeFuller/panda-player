using System.Collections.Generic;
using System.Threading.Tasks;
using MusicLibrary.DiscAdder.AddingToLibrary;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IAddToLibraryViewModel : IPageViewModel
	{
		void SetSongs(IEnumerable<AddedSong> songs);

		void SetDiscsImages(IEnumerable<AddedDiscImage> images);

		Task AddContentToLibrary();
	}
}
