using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IFoldersService
	{
		Task<FolderModel> GetRootFolder(CancellationToken cancellationToken);

		Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken);

		// TODO: Do we need this method or can adjust callers to use GetFolder()?
		Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken);
	}
}
