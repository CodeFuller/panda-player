using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IFoldersService
	{
		Task CreateFolder(ShallowFolderModel folder, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ShallowFolderModel>> GetAllFolders(CancellationToken cancellationToken);

		Task<FolderModel> GetRootFolder(CancellationToken cancellationToken);

		Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken);

		Task DeleteFolder(ItemId folderId, CancellationToken cancellationToken);
	}
}
