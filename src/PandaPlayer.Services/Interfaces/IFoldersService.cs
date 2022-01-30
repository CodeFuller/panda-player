using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IFoldersService
	{
		Task CreateEmptyFolder(FolderModel folder, CancellationToken cancellationToken);

		Task<FolderModel> GetRootFolder(CancellationToken cancellationToken);

		Task UpdateFolder(FolderModel folder, Action<FolderModel> updateAction, CancellationToken cancellationToken);

		Task DeleteEmptyFolder(FolderModel folder, CancellationToken cancellationToken);
	}
}
