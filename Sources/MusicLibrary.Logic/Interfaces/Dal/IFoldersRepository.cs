using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IFoldersRepository
	{
		Task<FolderModel> GetRootFolder(bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderModel> GetFolder(ItemId folderId, bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken);
	}
}
