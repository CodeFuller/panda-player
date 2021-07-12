using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.DiscAdder.AddedContent;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IAddToLibraryViewModel : IPageViewModel
	{
		void SetSongs(IEnumerable<AddedSong> songs);

		void SetDiscsImages(IEnumerable<AddedDiscImage> images);

		Task AddContentToLibrary(CancellationToken cancellationToken);
	}
}
