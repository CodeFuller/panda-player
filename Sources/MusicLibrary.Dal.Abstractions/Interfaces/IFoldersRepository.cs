using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Dal.Abstractions.Dto;
using MusicLibrary.Dal.Abstractions.Dto.Folders;

namespace MusicLibrary.Dal.Abstractions.Interfaces
{
	public interface IFoldersRepository
	{
		Task<FolderData> GetRootFolder(bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderData> GetFolder(ItemId folderId, bool includeDeletedDiscs, CancellationToken cancellationToken);

		Task<FolderData> GetDiscFolder(ItemId discId, CancellationToken cancellationToken);
	}
}
