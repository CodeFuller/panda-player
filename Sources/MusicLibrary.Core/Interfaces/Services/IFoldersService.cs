using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Services
{
	public interface IFoldersService
	{
		Task<FolderModel> GetRootFolder(CancellationToken cancellationToken);

		Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken);

		Task<FolderModel> GetDiscFolder(ItemId discId, CancellationToken cancellationToken);
	}
}
