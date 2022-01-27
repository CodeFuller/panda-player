using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IFoldersRepository
	{
		Task CreateFolder(ShallowFolderModel folder, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ShallowFolderModel>> GetFoldersWithoutDiscs(CancellationToken cancellationToken);

		Task UpdateFolder(ShallowFolderModel folder, CancellationToken cancellationToken);
	}
}
