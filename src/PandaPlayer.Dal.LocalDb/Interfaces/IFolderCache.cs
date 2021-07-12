using PandaPlayer.Core.Models;

namespace PandaPlayer.Dal.LocalDb.Interfaces
{
	public interface IFolderCache
	{
		void StoreFolder(ShallowFolderModel folder);
	}
}
