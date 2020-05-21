using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Services
{
	public interface IFoldersService
	{
		Task<FolderModel> GetRootFolder(bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderModel> GetFolder(ItemId folderId, bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken);
	}
}
