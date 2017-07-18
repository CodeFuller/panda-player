using System.Collections.Generic;
using System.Threading.Tasks;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces
{
	public interface IAddToLibraryViewModel
	{
		Task<bool> AddAlbumsToLibrary(IEnumerable<AlbumTreeViewItem> albums);
	}
}
