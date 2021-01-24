using MusicLibrary.Core.Models;

namespace MusicLibrary.Dal.LocalDb.Interfaces
{
	public interface IFolderCache
	{
		void StoreFolder(ShallowFolderModel folder);
	}
}
